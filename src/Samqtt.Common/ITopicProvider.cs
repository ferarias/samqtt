namespace Samqtt
{
    public interface ITopicProvider
    {
        public string StatusTopic { get; }
        public string UnsubscribeTopic { get; }

        public string GetUniqueId(string name);
        public string GetSensorStateTopic(string sensorName);
        public string GetSensorDiscoveryTopic(string sensorName, bool isBinary);

        public string GetActionStateTopic(string sensorName);
        public string GetActionDiscoveryTopic(string sensorName);
        public string GetActionCommandTopic(string actionName);
        public string GetActionJsonAttributesTopic(string actionName);
    }

}
