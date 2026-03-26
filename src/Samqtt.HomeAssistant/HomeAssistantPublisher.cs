using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samqtt.Options;
using Samqtt.SystemActions;
using Samqtt.SystemSensors;
using System.Text.Json;

namespace Samqtt.HomeAssistant
{
    public class HomeAssistantPublisher(
        IMqttPublisher mqttPublisher,
        ITopicProvider topicProvider,
        IOptionsMonitor<SamqttOptions> options,
        ILogger<HomeAssistantPublisher> logger)
        : IMessagePublisher
    {
        private readonly DeviceInfoPayload DeviceInfo = new(
            identifiers: [$"{Constants.AppId.ToLowerInvariant()}_{options.CurrentValue?.DeviceIdentifier}"],
            name: $"{Constants.AppId.ToLowerInvariant()} - {options.CurrentValue?.DeviceIdentifier}",
            manufacturer: "FerArias",
            model: "Generic Computer");

        public async Task PublishOnlineStatus(CancellationToken cancellationToken = default)
        {
            await mqttPublisher.PublishAsync(topicProvider.StatusTopic, "online", retain: true, atLeastOnce: true, cancellationToken);
            logger.LogDebug("Published online status");
        }

        public async Task PublishOfflineStatus(CancellationToken cancellationToken = default)
        {
            await mqttPublisher.PublishAsync(topicProvider.StatusTopic, "offline", retain: true, atLeastOnce: true, cancellationToken: cancellationToken);
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
            string discoveryTopic = metadata.DiscoveryTopic;

            var payload = new SensorDiscoveryPayload(
                name: metadata.Name,
                state_topic: metadata.StateTopic,
                unique_id: metadata.UniqueId,
                availability_topic: topicProvider.StatusTopic,
                device: DeviceInfo,
                unit_of_measurement: string.IsNullOrWhiteSpace(metadata?.UnitOfMeasurement) ? null : metadata.UnitOfMeasurement,
                state_class: string.IsNullOrWhiteSpace(metadata?.StateClass) ? null : metadata.StateClass,
                device_class: string.IsNullOrWhiteSpace(metadata?.DeviceClass) ? null : metadata.DeviceClass,
                payload_on: (metadata?.IsBinary == true && !string.IsNullOrWhiteSpace(metadata?.PayloadOn)) ? metadata.PayloadOn : null,
                payload_off: (metadata?.IsBinary == true && !string.IsNullOrWhiteSpace(metadata?.PayloadOff)) ? metadata.PayloadOff : null);

            await mqttPublisher.PublishAsync(
                discoveryTopic,
                JsonSerializer.Serialize(payload, SamqttHomeAssistantJsonContext.WithNullIgnore.SensorDiscoveryPayload),
                retain: true,
                atLeastOnce: true,
                cancellationToken: cancellationToken);

            logger.LogInformation("Published HA sensor discovery message for {Sensor} in {Topic}", metadata?.Key, discoveryTopic);

        }

        public async Task PublishActionStateDiscoveryMessage(SystemActionMetadata metadata, CancellationToken cancellationToken = default)
        {
            if (metadata.StateTopic == null || metadata.JsonAttributesTopic == null)
            {
                logger.LogInformation("Action {Action} does not return anything and therefore has no state topic", metadata?.Key);
                return;
            }
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

            var actionPayload = new ActionDiscoveryPayload(
                name: $"{metadata.Name} result",
                state_topic: metadata.StateTopic,
                json_attributes_topic: metadata.JsonAttributesTopic,
                unique_id: metadata.UniqueId,
                device: DeviceInfo);

            var discoveryTopic = topicProvider.GetActionResponseDiscoveryTopic(metadata.Key);
            await mqttPublisher.PublishAsync(
                discoveryTopic,
                JsonSerializer.Serialize(actionPayload, SamqttHomeAssistantJsonContext.WithNullIgnore.ActionDiscoveryPayload),
                retain: true,
                atLeastOnce: true,
                cancellationToken: cancellationToken);

            logger.LogInformation("Published HA action discovery message for {Action} in {Topic}", metadata?.Key, discoveryTopic);
        }
    }
}
