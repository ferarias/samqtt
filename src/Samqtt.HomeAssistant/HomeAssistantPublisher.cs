using Microsoft.Extensions.Logging;
using Samqtt.SystemActions;
using Samqtt.SystemSensors;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Samqtt.HomeAssistant
{
    public class HomeAssistantPublisher(
        IMqttPublisher mqttPublisher,
        ITopicProvider topicProvider,
        ISystemSensorValueFormatter sensorValueFormatter,
        ILogger<HomeAssistantPublisher> logger)
        : IMessagePublisher
    {

        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private readonly object DeviceInfo = new
        {
            identifiers = new[] { topicProvider.DeviceIdentifier },
            name = $"SAMQTT - { topicProvider.DeviceIdentifier }",
            manufacturer = "FerArias",
            model = "SAMQTT"
        };

        public async Task PublishOnlineStatus(CancellationToken cancellationToken = default)
        {
            await mqttPublisher.PublishAsync(topicProvider.StatusTopic, "online", retain: true, cancellationToken);
            logger.LogDebug("Published online status");
        }

        public async Task PublishOfflineStatus(CancellationToken cancellationToken = default)
        {
            await mqttPublisher.PublishAsync(topicProvider.StatusTopic, "offline", retain: true, cancellationToken: cancellationToken);
            logger.LogDebug("Published offline status");
        }

        public async Task PublishSensorValue(ISystemSensor sensor, object? value, CancellationToken cancellationToken = default)
        {
            if (sensor.Metadata.StateTopic == null)
            {
                logger.LogWarning("Sensor {Sensor} has no state topic", sensor.Metadata.Key);
                return;
            }
            try
            {
                await mqttPublisher.PublishAsync(sensor.Metadata.StateTopic, sensorValueFormatter.Format(value), false, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to publish sensor {Sensor}", sensor.Metadata.Key);
            }
        }

        public async Task PublishSensorDiscoveryMessage(SystemSensorMetadata metadata, CancellationToken cancellationToken = default)
        {
            if (metadata.StateTopic == null)
            {
                logger.LogWarning("Sensor {Sensor} has no state topic", metadata.Key);
                return;
            }
            if (string.IsNullOrWhiteSpace(metadata.UniqueId))
            {
                logger.LogWarning("Sensor {Sensor} has no unique ID", metadata.Key);
                return;
            }
            // Build the common payload dictionary
            var payloadDict = new Dictionary<string, object>
            {
                ["name"] = metadata.Name,
                ["state_topic"] = metadata.StateTopic,
                ["unique_id"] = metadata.UniqueId,
                ["availability_topic"] = topicProvider.StatusTopic,
                ["device"] = DeviceInfo
            };

            if (!string.IsNullOrWhiteSpace(metadata?.UnitOfMeasurement)) payloadDict["unit_of_measurement"] = metadata.UnitOfMeasurement;
            if (!string.IsNullOrWhiteSpace(metadata?.StateClass)) payloadDict["state_class"] = metadata.StateClass;
            if (!string.IsNullOrWhiteSpace(metadata?.DeviceClass)) payloadDict["device_class"] = metadata.DeviceClass;

            string? discoveryTopic;
            if (metadata?.IsBinary == true)
            {
                if (!string.IsNullOrWhiteSpace(metadata?.PayloadOn)) payloadDict["payload_on"] = metadata.PayloadOn;
                if (!string.IsNullOrWhiteSpace(metadata?.PayloadOff)) payloadDict["payload_off"] = metadata.PayloadOff;

                discoveryTopic = $"{HomeAssistantTopics.BaseTopic}/binary_sensor/{metadata.UniqueId}/config";
            }
            else
            {
                discoveryTopic = $"{HomeAssistantTopics.BaseTopic}/sensor/{metadata?.UniqueId}/config";
            }



            await mqttPublisher.PublishAsync(
                discoveryTopic,
                JsonSerializer.Serialize(payloadDict, jsonSerializerOptions),
                retain: true,
                cancellationToken: cancellationToken);

            logger.LogInformation("Published HA sensor discovery message for {Sensor} in {Topic}", metadata?.Key, discoveryTopic);

        }

        public async Task PublishSwitchDiscoveryMessage(SystemActionMetadata metadata, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(metadata?.CommandTopic))
            {
                logger.LogWarning("Switch {Switch} has no command topic", metadata?.Key);
                return;
            }
            if (string.IsNullOrWhiteSpace(metadata?.UniqueId))
            {
                logger.LogWarning("Switch {Switch} has no unique ID", metadata?.Key);
                return;
            }

            var payloadDict = new Dictionary<string, object>
            {
                ["name"] = metadata.Name,
                ["state_topic"] = metadata.StateTopic,
                ["unique_id"] = metadata.UniqueId,
                ["availability_topic"] = topicProvider.StatusTopic,
                ["command_topic"] = metadata.CommandTopic,
                ["payload_on"] = "ON",
                ["payload_off"] = "OFF",
                ["device"] = DeviceInfo
            };

            var discoveryTopic = $"{HomeAssistantTopics.BaseTopic}/switch/{metadata.UniqueId}/config";

            await mqttPublisher.PublishAsync(
                discoveryTopic,
                JsonSerializer.Serialize(payloadDict, jsonSerializerOptions),
                retain: true,
                cancellationToken: cancellationToken);

            logger.LogInformation("Published HA switch discovery message for {Switch} in {Topic}", metadata?.Key, discoveryTopic);
        }
    }
}
