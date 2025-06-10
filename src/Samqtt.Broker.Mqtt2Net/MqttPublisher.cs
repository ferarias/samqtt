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

        public Task PublishAsync((string Topic, string Payload) message, bool retain, CancellationToken cancellationToken = default) => 
            PublishAsync(message.Topic, message.Payload, retain, cancellationToken);

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