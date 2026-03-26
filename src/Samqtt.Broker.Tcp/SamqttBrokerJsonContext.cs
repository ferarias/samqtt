using System.Text.Json.Serialization;

namespace Samqtt.Broker.Tcp;

[JsonSerializable(typeof(ActionResultPayload))]
internal partial class SamqttBrokerJsonContext : JsonSerializerContext { }
