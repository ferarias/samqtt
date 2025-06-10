using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using Samqtt.Options;

namespace Samqtt.Broker.MQTTNet
{
    public class MqttConnector(
        IMqttClient client,
        IMqttPublisher publisher,
        IOptions<SamqttOptions> options,
        ILogger<MqttConnector> logger) : IMqttConnectionManager, IAsyncDisposable
    {
        private readonly IMqttClient _client = client;
        private readonly IMqttPublisher publisher = publisher;

        private readonly SamqttOptions _options = options.Value;
        private readonly ILogger _logger = logger;

        public bool IsConnected => _client.IsConnected;

        public async Task<bool> ConnectAsync(CancellationToken cancellationToken)
        {
            do
            {
                _logger.LogDebug("Connecting to MQTT broker.");
                try
                {
                    var mqttOptionsBuilder = new MqttClientOptionsBuilder()
                        .WithTcpServer(_options.Broker.Server, _options.Broker.Port)
                        .WithClientId(Guid.NewGuid().ToString())
                        .WithCleanSession();
                    if (!string.IsNullOrWhiteSpace(_options.Broker.Username) || !string.IsNullOrWhiteSpace(_options.Broker.Password))
                    {
                        mqttOptionsBuilder.WithCredentials(_options.Broker.Username, _options.Broker.Password);
                    }

                    var mqttClientOptions = mqttOptionsBuilder
                        .WithWillTopic($"{options.Value.MqttBaseTopic}/status")
                        .WithWillPayload("offline")
                        .WithWillRetain(true)
                        .Build();

                    var response = await _client.ConnectAsync(mqttClientOptions, cancellationToken);
                    if (response.ResultCode != MqttClientConnectResultCode.Success)
                    {
                        _logger.LogError("Could not connect to MQTT broker. Result code: {ResultCode}", response.ResultCode);
                    }
                    else
                    {
                        _logger.LogInformation("The MQTT client is connected.");
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Could not connect; check connection settings");
                }

                logger.LogWarning("MQTT connection failed. Retrying in 10 seconds...");
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);
            }
            while (!cancellationToken.IsCancellationRequested && !_client.IsConnected);
            return false;
        }

        public async Task DisconnectAsync(CancellationToken cancellationToken = default)
        {
            if (_client?.IsConnected == true)
            {
                var unsubscribeOptions = new MqttClientUnsubscribeOptionsBuilder()
                    .WithTopicFilter($"{options.Value.MqttBaseTopic}/#")
                    .Build();
                await _client.UnsubscribeAsync(unsubscribeOptions, cancellationToken);
                _logger.LogInformation("Unsubscribed from MQTT topics.");

                var disconnectOptions = new MqttClientDisconnectOptionsBuilder()
                    .WithReason(MqttClientDisconnectOptionsReason.NormalDisconnection)
                    .Build();
                await _client.DisconnectAsync(disconnectOptions, cancellationToken);
                _logger.LogInformation("The MQTT client is disconnected.");
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
}