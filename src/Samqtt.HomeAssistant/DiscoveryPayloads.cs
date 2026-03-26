namespace Samqtt.HomeAssistant
{
    internal sealed record DeviceInfoPayload(
        string[] identifiers,
        string name,
        string manufacturer,
        string model);

    internal sealed record SensorDiscoveryPayload(
        string name,
        string state_topic,
        string unique_id,
        string availability_topic,
        DeviceInfoPayload device,
        string? unit_of_measurement = null,
        string? state_class = null,
        string? device_class = null,
        string? payload_on = null,
        string? payload_off = null);

    internal sealed record ActionDiscoveryPayload(
        string name,
        string state_topic,
        string json_attributes_topic,
        string unique_id,
        DeviceInfoPayload device);
}
