using System.Text.Json.Serialization;

namespace Samqtt.Broker.MQTTNet
{
    [JsonSerializable(typeof(ActionResultPayload))]
    internal partial class SamqttBrokerJsonContext : JsonSerializerContext { }
}
