using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MQTTnet;
using MQTTnet.Protocol;
using Samqtt.Options;
using Samqtt.SystemActions;
using Samqtt.SystemSensors;

namespace Samqtt.Broker.MQTTNet
{
    public class MqttSubscriber : IMqttSubscriber
    {
        private readonly Dictionary<string, Func<string, CancellationToken, Task<object?>>> _actions = [];
        private readonly Dictionary<string, (string StateTopic, string JsonAttributesTopic)> _returnTopics = [];

        private readonly IMqttClient _client;
        private readonly SamqttOptions _options;
        private readonly ISystemActionFactory actionFactory;
        private readonly ILogger<MqttSubscriber> logger;

        public MqttSubscriber(
            IMqttClient client,
            IOptions<SamqttOptions> options,
            ISystemActionFactory actionFactory,
            IMessagePublisher publisher,
                ISystemSensorValueFormatter sensorValueFormatter,
            ILogger<MqttSubscriber> logger)
        {
            this.actionFactory = actionFactory;
            this.logger = logger;
            _client = client;
            _options = options.Value;

            // Define what happens when a commandPayload is received
            _client.ApplicationMessageReceivedAsync += async (e) =>
            {
                logger.LogDebug("Message received in MQTT commandTopic `{Topic}`.", e.ApplicationMessage.Topic);
                try
                {
                    var commandTopic = e.ApplicationMessage.Topic;
                    var commandPayload = e.ApplicationMessage.ConvertPayloadToString();

                    var result = await _actions[commandTopic].Invoke(commandPayload, CancellationToken.None);
                    var stateTopic = _returnTopics[commandTopic].StateTopic;
                    var jsonAttrTopic = _returnTopics[commandTopic].JsonAttributesTopic;

                    if (result is IEnumerable<object> enumerable)
                    {
                        await publisher.PublishActionStateValue(stateTopic, $"Returned {enumerable.Count()} items", CancellationToken.None);
                        var o = new
                        {
                            count = enumerable.Count(),
                            items = enumerable.Select(item => sensorValueFormatter.Format(item)).ToList()
                        };
                        await publisher.PublishActionStateValue(jsonAttrTopic, sensorValueFormatter.Format(o), CancellationToken.None);
                    }
                    else
                    {
                        var v = sensorValueFormatter.Format(result);
                        await publisher.PublishActionStateValue(stateTopic, v, CancellationToken.None);
                        await publisher.PublishActionStateValue(jsonAttrTopic, v, CancellationToken.None);
                    }

                    
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Message received MQTT commandTopic `{Topic}` caused an exception.", e.ApplicationMessage.Topic);
                }
            };
        }

        public async Task<bool> SubscribeAsync(SystemActionMetadata metadata, Func<string, CancellationToken, Task<object?>> handler, CancellationToken cancellationToken = default)
        {
            var commandTopic = metadata.CommandTopic;
            string stateTopic = metadata.StateTopic!;
            string jsonAttrTopic = metadata.JsonAttributesTopic!;
            do
            {
                try
                {
                    if (_client?.IsConnected != true) return false;

                    await _client.SubscribeAsync(commandTopic, MqttQualityOfServiceLevel.ExactlyOnce, cancellationToken);
                    logger.LogDebug("Subscribe to MQTT commandTopic `{Topic}`. Success.", commandTopic);

                    // Register the handler for this commandTopic
                    _actions.Add(commandTopic, handler);
                    // Keep track of the state and JSON Attributes topics associated to this commandTopic
                    _returnTopics.Add(commandTopic, (stateTopic, jsonAttrTopic));
                    return true;

                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "Subscribe to MQTT commandTopic `{Topic}`. Exception!", commandTopic);
                }

                logger.LogWarning("Subscribe to MQTT commandTopic `{Topic}`. Could not subscribe; retrying in 10 seconds...", commandTopic);
                await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            } while (!cancellationToken.IsCancellationRequested);

            return false;
        }
    }
}