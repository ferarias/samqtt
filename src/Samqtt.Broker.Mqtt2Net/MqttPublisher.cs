using System.Text;
using Microsoft.Extensions.Logging;
using MQTTnet;
using MQTTnet.Protocol;

namespace Samqtt.Broker.MQTTNet
{
    public class MqttPublisher(IMqttClient client, ILogger<MqttPublisher> logger) : IMqttPublisher
    {
        private readonly IMqttClient _client = client;
        private readonly ILogger<MqttPublisher> _logger = logger;


        public async Task PublishAsync(string topic, string message, bool retain = false, CancellationToken cancellationToken = default)
        {
            if (_client.IsConnected)
            {
                var mqttMessage = new MqttApplicationMessageBuilder()
                    .WithTopic(topic)
                    .WithPayload(Encoding.UTF8.GetBytes(message))
                    .WithRetainFlag(retain)
                    .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
                .Build();

                await _client.PublishAsync(mqttMessage, cancellationToken);
                _logger.LogTrace("Message published: {Topic} value {Message}", topic, message);
            }
        }
    }
}