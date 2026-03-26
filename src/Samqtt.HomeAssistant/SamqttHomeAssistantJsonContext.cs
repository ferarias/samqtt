using System.Text.Json.Serialization;
using System.Text.Json;

namespace Samqtt.HomeAssistant
{
    [JsonSerializable(typeof(SensorDiscoveryPayload))]
    [JsonSerializable(typeof(ActionDiscoveryPayload))]
    [JsonSerializable(typeof(DeviceInfoPayload))]
    internal partial class SamqttHomeAssistantJsonContext : JsonSerializerContext
    {
        private static readonly JsonSerializerOptions _options = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        };

        public static SamqttHomeAssistantJsonContext WithNullIgnore { get; } = new(_options);
    }
}
