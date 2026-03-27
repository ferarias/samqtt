using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Samqtt.Broker.Tcp;

internal sealed class MqttTcpClient(ILogger<MqttTcpClient> logger) : IAsyncDisposable
{
    private TcpClient? _tcpClient;
    private NetworkStream? _stream;
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private readonly ConcurrentDictionary<ushort, TaskCompletionSource<bool>> _pendingAcks = new();
    private int _nextPacketId;
    private volatile bool _isConnected;
    private CancellationTokenSource? _loopCts;

    public bool IsConnected => _isConnected;

    // Invoked by the read loop when a PUBLISH packet arrives from the broker.
    // Subscribe before calling ConnectAsync to avoid missing messages.
    public Func<string, string, Task>? MessageReceived;

    public async Task ConnectAsync(
        string host,
        int? port,
        string clientId,
        (string Username, string Password)? credentials,
        (string Topic, string Payload, bool Retain) will,
        int keepaliveSecs,
        CancellationToken cancellationToken)
    {
        // Clean up any previous connection
        _loopCts?.Cancel();
        _loopCts?.Dispose();
        _loopCts = null;
        _isConnected = false;
        _pendingAcks.Clear();
        _stream?.Dispose();
        _tcpClient?.Dispose();

        // Resolve hostname and prefer IPv4 to avoid connecting to the wrong endpoint
        // when "localhost" resolves to both ::1 (IPv6) and 127.0.0.1 (IPv4) on Windows.
        var addresses = await Dns.GetHostAddressesAsync(host, cancellationToken);
        var address = addresses
            .OrderByDescending(a => a.AddressFamily == AddressFamily.InterNetwork)
            .First();
        logger.LogDebug("Resolved {Host} to {Address} ({AddressFamily})", host, address, address.AddressFamily);
        _tcpClient = new TcpClient(address.AddressFamily);
        await _tcpClient.ConnectAsync(address, port ?? 1883, cancellationToken);
        _stream = _tcpClient.GetStream();

        await SendConnectAsync(clientId, credentials, will, keepaliveSecs, cancellationToken);
        await ReadConnackAsync(cancellationToken);

        _isConnected = true;
        _loopCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        _ = Task.Run(() => ReadLoopAsync(_loopCts.Token), _loopCts.Token);
        _ = Task.Run(() => KeepaliveLoopAsync(keepaliveSecs, _loopCts.Token), _loopCts.Token);
    }

    public async Task PublishAsync(
        string topic,
        ReadOnlyMemory<byte> payload,
        bool retain,
        bool atLeastOnce,
        CancellationToken cancellationToken)
    {
        if (!_isConnected) return;

        if (!atLeastOnce)
        {
            await WritePacketAsync(BuildPublish(topic, payload, retain, qos: 0, packetId: 0), cancellationToken);
            return;
        }

        var packetId = NextPacketId();
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pendingAcks[packetId] = tcs;
        try
        {
            await WritePacketAsync(BuildPublish(topic, payload, retain, qos: 1, packetId), cancellationToken);
            await tcs.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
        }
        finally
        {
            _pendingAcks.TryRemove(packetId, out _);
        }
    }

    public async Task SubscribeAsync(string topic, CancellationToken cancellationToken)
    {
        var packetId = NextPacketId();
        var tcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
        _pendingAcks[packetId] = tcs;
        try
        {
            await WritePacketAsync(BuildSubscribe(topic, qos: 1, packetId), cancellationToken);
            await tcs.Task.WaitAsync(TimeSpan.FromSeconds(30), cancellationToken);
        }
        finally
        {
            _pendingAcks.TryRemove(packetId, out _);
        }
    }

    public async Task UnsubscribeAsync(string topic, CancellationToken cancellationToken)
    {
        if (!_isConnected) return;
        await WritePacketAsync(BuildUnsubscribe(topic, NextPacketId()), cancellationToken);
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        _isConnected = false;

        // Unblock any QoS-1 publish/subscribe callers waiting for an ack so they don't
        // sit for up to 30 seconds after the connection is gone.
        foreach (var tcs in _pendingAcks.Values)
            tcs.TrySetCanceled(CancellationToken.None);
        _pendingAcks.Clear();

        try
        {
            await WritePacketAsync([0xE0, 0x00], cancellationToken);
        }
        catch { /* ignore write errors during disconnect */ }
        _loopCts?.Cancel();
        _stream?.Dispose();
        _tcpClient?.Dispose();
        _stream = null;
        _tcpClient = null;
    }

    private async Task ReadLoopAsync(CancellationToken ct)
    {
        try
        {
            while (!ct.IsCancellationRequested)
            {
                var (type, body) = await ReadPacketAsync(ct);
                switch (type >> 4)
                {
                    case 3: // PUBLISH
                        await HandlePublishAsync(type, body, ct);
                        break;
                    case 4: // PUBACK
                        if (_pendingAcks.TryGetValue(ReadUInt16(body, 0), out var pubackTcs))
                            pubackTcs.TrySetResult(true);
                        break;
                    case 9: // SUBACK
                        if (_pendingAcks.TryGetValue(ReadUInt16(body, 0), out var subackTcs))
                            subackTcs.TrySetResult(true);
                        break;
                    case 11: // UNSUBACK
                    case 13: // PINGRESP
                        break;
                    default:
                        logger.LogDebug("Unhandled MQTT packet type {Type}", type >> 4);
                        break;
                }
            }
        }
        catch (OperationCanceledException) { }
        catch (IOException) { }
        catch (ObjectDisposedException) { }
        catch (Exception ex)
        {
            logger.LogError(ex, "MQTT read loop terminated unexpectedly");
        }
        finally
        {
            _isConnected = false;
        }
    }

    private async Task HandlePublishAsync(byte fixedHeader, byte[] body, CancellationToken ct)
    {
        var qos = (fixedHeader >> 1) & 0x03;
        var offset = 0;
        var topicLen = ReadUInt16(body, offset); offset += 2;
        var topic = Encoding.UTF8.GetString(body, offset, topicLen); offset += topicLen;

        ushort packetId = 0;
        if (qos > 0) { packetId = ReadUInt16(body, offset); offset += 2; }

        var payload = Encoding.UTF8.GetString(body, offset, body.Length - offset);

        try
        {
            if (MessageReceived != null)
                await MessageReceived(topic, payload);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Message handler for topic '{Topic}' threw an exception", topic);
        }

        if (qos == 1)
            await WritePacketAsync([0x40, 0x02, (byte)(packetId >> 8), (byte)(packetId & 0xFF)], ct);
    }

    private async Task KeepaliveLoopAsync(int keepaliveSecs, CancellationToken ct)
    {
        var interval = TimeSpan.FromSeconds(Math.Max(1, keepaliveSecs - 5));
        using var timer = new PeriodicTimer(interval);
        try
        {
            while (await timer.WaitForNextTickAsync(ct))
            {
                if (_isConnected)
                    await WritePacketAsync([0xC0, 0x00], ct);
            }
        }
        catch (OperationCanceledException) { }
        catch (Exception ex)
        {
            logger.LogError(ex, "MQTT keepalive loop terminated unexpectedly");
        }
    }

    private async Task SendConnectAsync(
        string clientId,
        (string Username, string Password)? credentials,
        (string Topic, string Payload, bool Retain) will,
        int keepaliveSecs,
        CancellationToken ct)
    {
        using var body = new MemoryStream();
        WriteString(body, "MQTT");  // Protocol Name
        body.WriteByte(4);          // Protocol Level: MQTT 3.1.1

        byte flags = 0x02;          // CleanSession
        flags |= 0x04;              // Will Flag
        flags |= 0x08;              // Will QoS = 1 (bit 3)
        if (will.Retain) flags |= 0x20;
        if (credentials.HasValue) { flags |= 0x80; flags |= 0x40; } // Username + Password present

        body.WriteByte(flags);
        body.WriteByte((byte)(keepaliveSecs >> 8));
        body.WriteByte((byte)(keepaliveSecs & 0xFF));

        WriteString(body, clientId);
        WriteString(body, will.Topic);
        WriteString(body, will.Payload);
        if (credentials.HasValue)
        {
            WriteString(body, credentials.Value.Username);
            WriteString(body, credentials.Value.Password);
        }

        var bodyBytes = body.ToArray();
        using var packet = new MemoryStream();
        packet.WriteByte(0x10);
        WriteVarint(packet, bodyBytes.Length);
        packet.Write(bodyBytes);
        await WritePacketAsync(packet.ToArray(), ct);
    }

    private async Task ReadConnackAsync(CancellationToken ct)
    {
        var (type, body) = await ReadPacketAsync(ct);
        if ((type >> 4) != 2)
            throw new InvalidOperationException($"Expected CONNACK (type 2), received packet type {type >> 4}");
        if (body.Length < 2)
            throw new InvalidOperationException("Malformed CONNACK: too short");
        if (body[1] != 0)
            throw new InvalidOperationException($"MQTT broker refused connection, return code: {body[1]}");
    }

    private async Task<(byte Type, byte[] Body)> ReadPacketAsync(CancellationToken ct)
    {
        var stream = _stream ?? throw new InvalidOperationException("Not connected");
        var typeByte = await ReadByteAsync(stream, ct);

        int remainingLength = 0, multiplier = 1;
        byte b;
        do
        {
            b = await ReadByteAsync(stream, ct);
            remainingLength += (b & 0x7F) * multiplier;
            multiplier *= 128;
            if (multiplier > 128 * 128 * 128)
                throw new InvalidOperationException("Malformed MQTT remaining length");
        } while ((b & 0x80) != 0);

        var body = new byte[remainingLength];
        var totalRead = 0;
        while (totalRead < remainingLength)
            totalRead += await stream.ReadAsync(body.AsMemory(totalRead), ct);

        return (typeByte, body);
    }

    private static async Task<byte> ReadByteAsync(NetworkStream stream, CancellationToken ct)
    {
        var buf = new byte[1];
        if (await stream.ReadAsync(buf, ct) == 0)
            throw new EndOfStreamException("MQTT broker closed the connection");
        return buf[0];
    }

    private async Task WritePacketAsync(byte[] data, CancellationToken ct)
    {
        await _writeLock.WaitAsync(ct);
        try
        {
            if (_stream == null) return;
            await _stream.WriteAsync(data, ct);
        }
        finally
        {
            _writeLock.Release();
        }
    }

    private static byte[] BuildPublish(string topic, ReadOnlyMemory<byte> payload, bool retain, int qos, ushort packetId)
    {
        var topicBytes = Encoding.UTF8.GetBytes(topic);
        var bodyLen = 2 + topicBytes.Length + (qos > 0 ? 2 : 0) + payload.Length;

        using var ms = new MemoryStream();
        ms.WriteByte((byte)(0x30 | (qos << 1) | (retain ? 1 : 0)));
        WriteVarint(ms, bodyLen);
        ms.WriteByte((byte)(topicBytes.Length >> 8));
        ms.WriteByte((byte)(topicBytes.Length & 0xFF));
        ms.Write(topicBytes);
        if (qos > 0)
        {
            ms.WriteByte((byte)(packetId >> 8));
            ms.WriteByte((byte)(packetId & 0xFF));
        }
        ms.Write(payload.Span);
        return ms.ToArray();
    }

    private static byte[] BuildSubscribe(string topic, int qos, ushort packetId)
    {
        var topicBytes = Encoding.UTF8.GetBytes(topic);
        var bodyLen = 2 + 2 + topicBytes.Length + 1;

        using var ms = new MemoryStream();
        ms.WriteByte(0x82); // SUBSCRIBE with required flags (bits 1-0 = 10)
        WriteVarint(ms, bodyLen);
        ms.WriteByte((byte)(packetId >> 8));
        ms.WriteByte((byte)(packetId & 0xFF));
        ms.WriteByte((byte)(topicBytes.Length >> 8));
        ms.WriteByte((byte)(topicBytes.Length & 0xFF));
        ms.Write(topicBytes);
        ms.WriteByte((byte)qos);
        return ms.ToArray();
    }

    private static byte[] BuildUnsubscribe(string topic, ushort packetId)
    {
        var topicBytes = Encoding.UTF8.GetBytes(topic);
        var bodyLen = 2 + 2 + topicBytes.Length;

        using var ms = new MemoryStream();
        ms.WriteByte(0xA2); // UNSUBSCRIBE with required flags (bits 1-0 = 10)
        WriteVarint(ms, bodyLen);
        ms.WriteByte((byte)(packetId >> 8));
        ms.WriteByte((byte)(packetId & 0xFF));
        ms.WriteByte((byte)(topicBytes.Length >> 8));
        ms.WriteByte((byte)(topicBytes.Length & 0xFF));
        ms.Write(topicBytes);
        return ms.ToArray();
    }

    private static void WriteString(Stream stream, string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);
        stream.WriteByte((byte)(bytes.Length >> 8));
        stream.WriteByte((byte)(bytes.Length & 0xFF));
        stream.Write(bytes);
    }

    private static void WriteVarint(Stream stream, int value)
    {
        do
        {
            byte b = (byte)(value & 0x7F);
            value >>= 7;
            if (value > 0) b |= 0x80;
            stream.WriteByte(b);
        } while (value > 0);
    }

    private static ushort ReadUInt16(byte[] data, int offset) =>
        (ushort)((data[offset] << 8) | data[offset + 1]);

    private ushort NextPacketId()
    {
        int id;
        do { id = Interlocked.Increment(ref _nextPacketId) & 0xFFFF; }
        while (id == 0);
        return (ushort)id;
    }

    public async ValueTask DisposeAsync()
    {
        await DisconnectAsync();
        _writeLock.Dispose();
        _loopCts?.Dispose();
    }
}
