using Microsoft.Extensions.Options;
using MQTTnet.Extensions.TopicTemplate;
using Samqtt.Common;
using Samqtt.Options;

namespace Samqtt
{
    public interface ITopicProvider
    {
        public string DeviceIdentifier { get; }
        public string BaseTopic { get; }
        public string StatusTopic { get; }
        public string UnsubscribeTopic { get; }

        public string GetUniqueId(string name);
        public string GetStateTopic(string name);
    }

    public class TopicProvider(IOptions<SamqttOptions> options) : ITopicProvider
    {
        public static readonly string AppIdTopic = Constants.AppId.ToLowerInvariant();

        public string DeviceIdentifier => options.Value.DeviceIdentifier;
        public string BaseTopic { get; } = $"{AppIdTopic}/{options.Value.DeviceIdentifier}";
        public string StatusTopic { get; } = $"{AppIdTopic}/{options.Value.DeviceIdentifier}/status";
        public string UnsubscribeTopic { get; } = $"{AppIdTopic}/{options.Value.DeviceIdentifier}/#";

        public string GetUniqueId(string name) => $"{AppIdTopic}_{DeviceIdentifier}_{SanitizeHelpers.Sanitize(name)}";
        public string GetStateTopic(string name) => $"{AppIdTopic}/{options.Value.DeviceIdentifier}/{SanitizeHelpers.Sanitize(name)}";
    }

    public static class MqttTopicTemplates
    {
        public static readonly MqttTopicTemplate SensorState =
            new("samqtt/{device}/sensor/{sensor}/state");

        public static readonly MqttTopicTemplate ActionCommand =
            new("samqtt/{device}/switch/{action}/command");

        public static readonly MqttTopicTemplate Status =
            new("samqtt/{device}/status");

    }
}
