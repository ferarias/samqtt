using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samqtt.Options;

namespace Samqtt.Broker.Tcp;

internal class MqttConnector(
    MqttTcpClient client,
    ITopicProvider topicProvider,
    IOptions<SamqttOptions> options,
    ILogger<MqttConnector> logger) : IMqttConnectionManager, IAsyncDisposable
{
    private const int KeepaliveSecs = 60;
    private readonly SamqttOptions _options = options.Value;

    public bool IsConnected => client.IsConnected;

    public async Task<bool> ConnectAsync(CancellationToken cancellationToken)
    {
        do
        {
            logger.LogDebug("Connecting to MQTT broker.");
            try
            {
                (string, string)? credentials = null;
                if (!string.IsNullOrWhiteSpace(_options.Broker.Username) || !string.IsNullOrWhiteSpace(_options.Broker.Password))
                    credentials = (_options.Broker.Username ?? string.Empty, _options.Broker.Password ?? string.Empty);

                await client.ConnectAsync(
                    host: _options.Broker.Server,
                    port: _options.Broker.Port,
                    clientId: Guid.NewGuid().ToString(),
                    credentials: credentials,
                    will: (topicProvider.StatusTopic, "offline", Retain: true),
                    keepaliveSecs: KeepaliveSecs,
                    cancellationToken: cancellationToken);

                logger.LogInformation("The MQTT client is connected.");
                return true;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Could not connect; check connection settings");
            }

            logger.LogWarning("MQTT connection failed. Retrying in 10 seconds...");
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
        }
        while (!cancellationToken.IsCancellationRequested && !client.IsConnected);
        return false;
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken = default)
    {
        if (client.IsConnected)
        {
            await client.UnsubscribeAsync(topicProvider.UnsubscribeTopic, cancellationToken);
            logger.LogInformation("Unsubscribed from MQTT topics.");

            await client.DisconnectAsync(cancellationToken);
            logger.LogInformation("The MQTT client is disconnected.");
        }
    }

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        await DisconnectAsync();
    }
}
