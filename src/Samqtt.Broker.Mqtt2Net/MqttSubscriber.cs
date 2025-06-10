using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Protocol;
using Samqtt.Options;
using Samqtt.SystemActions;

namespace Samqtt.Broker.MQTTNet
{
    public class MqttSubscriber : IMqttSubscriber
    {
        private Dictionary<string, Func<string, CancellationToken, Task<object?>>> _actions = [];

        private readonly IMqttClient _client;
        private readonly SamqttOptions _options;
        private readonly ISystemActionFactory actionFactory;
        private readonly ILogger<MqttSubscriber> logger;

        public MqttSubscriber(
            IMqttClient client,
            IOptions<SamqttOptions> options,
            ISystemActionFactory actionFactory,
            ILogger<MqttSubscriber> logger)
        {
            this.actionFactory = actionFactory;
            this.logger = logger;
            _client = client;
            _options = options.Value;

            // Define what happens when a message is received
            _client.ApplicationMessageReceivedAsync += async (e) =>
            {
                logger.LogDebug("Message received in MQTT topic `{Topic}`.", e.ApplicationMessage.Topic);
                try
                {
                    var operation = e.ApplicationMessage.Topic;
                    var message = e.ApplicationMessage.ConvertPayloadToString();
                    await _actions[operation].Invoke(message, CancellationToken.None);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Message received MQTT topic `{Topic}` caused an exception.", e.ApplicationMessage.Topic);
                }
            };
        }

        public async Task<bool> SubscribeAsync(string topic, Func<string, CancellationToken, Task<object?>> handler, CancellationToken cancellationToken = default)
        {
            do
            {
                try
                {
                    if (_client?.IsConnected != true) return false;

                    await _client.SubscribeAsync(topic, MqttQualityOfServiceLevel.ExactlyOnce, cancellationToken);
                    logger.LogDebug("Subscribe to MQTT topic `{Topic}`. Success.", topic);

                    // Register the handler for this topic
                    _actions.Add(topic, handler);
                    return true;

                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Subscribe to MQTT topic `{Topic}`. Exception!", topic);
                }

                logger.LogWarning("Subscribe to MQTT topic `{Topic}`. Could not subscribe; retrying in 10 seconds...", topic);
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            } while (!cancellationToken.IsCancellationRequested);

            return false;
        }
    }
}