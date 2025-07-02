namespace Samqtt
{
    public interface ITopicProvider
    {
        public string StatusTopic { get; }
        public string UnsubscribeTopic { get; }

        public string GetUniqueId(string name);
        public string GetSensorStateTopic(string sensorName);
        public string GetBinarySensorDiscoveryTopic(string sensorName);
        public string GetStandardSensorDiscoveryTopic(string sensorName);
        string GetButtonDiscoveryTopic(string actionName);

        public string GetActionStateTopic(string sensorName);
        public string GetActionResponseDiscoveryTopic(string sensorName);
        public string GetActionCommandTopic(string actionName);
        public string GetActionJsonAttributesTopic(string actionName);
    }

}
