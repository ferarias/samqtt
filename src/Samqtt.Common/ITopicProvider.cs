using MQTTnet.Extensions.TopicTemplate;

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

}
