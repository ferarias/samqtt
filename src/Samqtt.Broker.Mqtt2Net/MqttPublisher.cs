using System.Buffers;
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


        public async Task PublishAsync(string topic, string message, bool retain = false, bool atLeastOnce = false, CancellationToken cancellationToken = default)
        {
            if (_client.IsConnected)
            {
                var qos = atLeastOnce
                    ? MqttQualityOfServiceLevel.AtLeastOnce
                    : MqttQualityOfServiceLevel.AtMostOnce;

                var maxByteCount = Encoding.UTF8.GetMaxByteCount(message.Length);
                var buffer = ArrayPool<byte>.Shared.Rent(maxByteCount);
                try
                {
                    var byteCount = Encoding.UTF8.GetBytes(message, buffer);
                    var mqttMessage = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(buffer[..byteCount])
                        .WithRetainFlag(retain)
                        .WithQualityOfServiceLevel(qos)
                    .Build();

                    await _client.PublishAsync(mqttMessage, cancellationToken);
                    _logger.LogTrace("Message published: {Topic} value {Message}", topic, message);
                }
                finally
                {
                    ArrayPool<byte>.Shared.Return(buffer);
                }
            }
        }
    }
}