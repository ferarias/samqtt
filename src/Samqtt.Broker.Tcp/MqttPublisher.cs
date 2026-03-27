using System.Buffers;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Samqtt.Broker.Tcp;

internal class MqttPublisher(MqttTcpClient client, ILogger<MqttPublisher> logger) : IMqttPublisher
{
    public async Task PublishAsync(string topic, string message, bool retain = false, bool atLeastOnce = false, CancellationToken cancellationToken = default)
    {
        if (!client.IsConnected)
            throw new InvalidOperationException("Cannot publish: MQTT client is not connected.");

        var maxByteCount = Encoding.UTF8.GetMaxByteCount(message.Length);
        var buffer = ArrayPool<byte>.Shared.Rent(maxByteCount);
        try
        {
            var byteCount = Encoding.UTF8.GetBytes(message, buffer);
            await client.PublishAsync(topic, buffer.AsMemory(0, byteCount), retain, atLeastOnce, cancellationToken);
            logger.LogTrace("Message published: {Topic} value {Message}", topic, message);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);
        }
    }
}
