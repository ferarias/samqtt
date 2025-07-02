using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samqtt.Options;
using Samqtt.SystemActions;
using Samqtt.SystemSensors;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Samqtt.HomeAssistant
{
    public class HomeAssistantPublisher(
        IMqttPublisher mqttPublisher,
        ITopicProvider topicProvider,
        IOptionsMonitor<SamqttOptions> options,
        ILogger<HomeAssistantPublisher> logger)
        : IMessagePublisher
    {

        private static readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private readonly object DeviceInfo = new
        {
            identifiers = new[] { Constants.AppId },
            name = $"{Constants.AppId} - {options.CurrentValue?.DeviceIdentifier}",
            manufacturer = "FerArias",
            model = "Generic Computer",
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

        public async Task PublishSensorValue(string stateTopic, string value, CancellationToken cancellationToken = default)
        {
            try
            {
                await mqttPublisher.PublishAsync(stateTopic, value, false, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to publish sensor state to {Topic}", stateTopic);
            }
        }

        public async Task PublishActionStateValue(string stateTopic, string value, CancellationToken cancellationToken = default)
        {
            try
            {
                await mqttPublisher.PublishAsync(stateTopic, value, false, cancellationToken: cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to publish action state to {Topic}", stateTopic);
            }
        }

        public async Task PublishSensorStateDiscoveryMessage(SystemSensorMetadata metadata, CancellationToken cancellationToken = default)
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
            string discoveryTopic = metadata.DiscoveryTopic;

            if (!string.IsNullOrWhiteSpace(metadata?.UnitOfMeasurement)) payloadDict["unit_of_measurement"] = metadata.UnitOfMeasurement;
            if (!string.IsNullOrWhiteSpace(metadata?.StateClass)) payloadDict["state_class"] = metadata.StateClass;
            if (!string.IsNullOrWhiteSpace(metadata?.DeviceClass)) payloadDict["device_class"] = metadata.DeviceClass;

            if (metadata?.IsBinary == true)
            {
                if (!string.IsNullOrWhiteSpace(metadata?.PayloadOn)) payloadDict["payload_on"] = metadata.PayloadOn;
                if (!string.IsNullOrWhiteSpace(metadata?.PayloadOff)) payloadDict["payload_off"] = metadata.PayloadOff;
            }

            await mqttPublisher.PublishAsync(
                discoveryTopic,
                JsonSerializer.Serialize(payloadDict, jsonSerializerOptions),
                retain: true,
                cancellationToken: cancellationToken);

            logger.LogInformation("Published HA sensor discovery message for {Sensor} in {Topic}", metadata?.Key, discoveryTopic);

        }

        public async Task PublishActionStateDiscoveryMessage(SystemActionMetadata metadata, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(metadata?.CommandTopic))
            {
                logger.LogWarning("Action {Action} has no command topic", metadata?.Key);
                return;
            }
            if (string.IsNullOrWhiteSpace(metadata?.UniqueId))
            {
                logger.LogWarning("Action {Action} has no unique ID", metadata?.Key);
                return;
            }

            var payloadDict = new Dictionary<string, object>
            {
                ["name"] = $"{metadata.Name} result",
                ["state_topic"] = metadata.StateTopic,
                ["json_attributes_topic"] = metadata.JsonAttributesTopic,
                ["unique_id"] = metadata.UniqueId,
                ["device"] = DeviceInfo
            };

            var discoveryTopic = topicProvider.GetActionResponseDiscoveryTopic(metadata.Key);
            await mqttPublisher.PublishAsync(
                discoveryTopic,
                JsonSerializer.Serialize(payloadDict, jsonSerializerOptions),
                retain: true,
                cancellationToken: cancellationToken);

            logger.LogInformation("Published HA action discovery message for {Action} in {Topic}", metadata?.Key, discoveryTopic);
        }
    }
}
