using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samqtt.Common;
using Samqtt.Options;
using Samqtt.SystemSensors;

namespace Samqtt.Application
{
    public class SystemSensorFactory(
        IOptions<SamqttOptions> options,
        IServiceProvider serviceProvider,
        ITopicProvider topicProvider,
        ILogger<SystemSensorFactory> logger) : ISystemSensorFactory
    {
        private readonly SamqttOptions _options = options.Value;

        public IEnumerable<ISystemSensor> GetEnabledSensors()
        {
            var allSensors = serviceProvider.GetServices<ISystemSensor>();
            var allMultiSensors = serviceProvider.GetServices<ISystemMultiSensor>();

            // SENSORS
            foreach (var (sensorName, sensorOptions) in _options.Sensors)
            {
                if (!sensorOptions.Enabled)
                {
                    logger.LogDebug("Sensor {Sensor} is disabled in configuration.", sensorName);
                    continue;
                }

                var sensorInstance = allSensors.FirstOrDefault(a => a.GetType().Name.Equals(sensorName + "Sensor", StringComparison.OrdinalIgnoreCase));
                if (sensorInstance == null)
                {
                    logger.LogWarning("No sensorInstance ISystemSensor implementation found for key: {Sensor}", sensorName);
                    continue;
                }

                sensorInstance.Metadata = CreateMetadata(sensorInstance.GetType(), sensorName, SanitizeTopicOrDefault(sensorName, sensorOptions.Topic));

                yield return sensorInstance;
            }

            // MULTI-SENSORS
            foreach (var (multisensorName, multisensorOptions) in _options.MultiSensors)
            {
                if (!multisensorOptions.Enabled)
                {
                    logger.LogDebug("MultiSensor {MultiSensor} is disabled in config.", multisensorName);
                    continue;
                }

                var multisensorInstance = allMultiSensors
                    .FirstOrDefault(a => a.GetType().Name.Equals(multisensorName + "MultiSensor", StringComparison.OrdinalIgnoreCase));


                if (multisensorInstance == null)
                {
                    logger.LogWarning("No multisensorInstance IMultiSystemSensor implementation found for key: {MultiSensor}", multisensorName);
                    continue;
                }

                foreach (var (childSensorName, childSensorOptions) in multisensorOptions.Sensors)
                {
                    if (!childSensorOptions.Enabled)
                    {
                        logger.LogDebug("Multi-sensor child sensor {Sensor} is disabled in config.", childSensorName);
                        continue;
                    }

                    var childSensorInstance = allSensors
                        .FirstOrDefault(a => a.GetType().Name.Equals(childSensorName + "Sensor", StringComparison.OrdinalIgnoreCase));

                    if (childSensorInstance == null)
                    {
                        logger.LogWarning("No childSensorInstance ISystemSensor implementation found for key: {Sensor}", childSensorName);
                        continue;
                    }

                    foreach (var childId in multisensorInstance.ChildIdentifiers)
                    {
                        var sensorName = $"{childSensorName}Sensor_{childId}";
                        var sensorInstance = serviceProvider.GetKeyedService<ISystemSensor>(sensorName);
                        if (sensorInstance is null)
                        {
                            logger.LogWarning(
                                "DI could not resolve child sensor `{Sensor}` (child sensor of {Parent})",
                                childSensorName, multisensorName
                            );
                            continue;
                        }

                        var childTopicName = string.Concat(
                            SanitizeTopicOrDefault(multisensorName, multisensorOptions.Topic), '_',
                            SanitizeTopicOrDefault(sensorName, childSensorOptions.Topic));

                        sensorInstance.Metadata = CreateMetadata(sensorInstance.GetType(), sensorName, childTopicName, childId);

                        yield return sensorInstance;
                    }

                }
            }
        }

        private SystemSensorMetadata CreateMetadata(Type sensorType, string sensorName, string topic, string? instanceId = null)
        {
            var sm = new SystemSensorMetadata
            {
                Key = sensorName,
                Name = sensorType.Name.Replace("Sensor", string.Empty),
                UniqueId = topicProvider.GetUniqueId(sensorName),
                StateTopic = topicProvider.GetStateTopic(topic),
                InstanceId = instanceId
            };
            if (Attribute.GetCustomAttribute(sensorType, typeof(HomeAssistantSensorAttribute)) is HomeAssistantSensorAttribute haAttr)
            {
                sm.UnitOfMeasurement = haAttr.UnitOfMeasurement;
                sm.DeviceClass = haAttr.DeviceClass;
                sm.StateClass = haAttr.StateClass;
            }
            if (Attribute.GetCustomAttribute(sensorType, typeof(HomeAssistantBinarySensorAttribute)) is HomeAssistantBinarySensorAttribute habAttr)
            {
                sm.IsBinary = true;
                sm.PayloadOn = habAttr.PayloadOn;
                sm.PayloadOff = habAttr.PayloadOff;
            }
            return sm;
        }

        private static string SanitizeTopicOrDefault(string fallback, string? topic) => 
            SanitizeHelpers.Sanitize(string.IsNullOrWhiteSpace(topic) ? fallback : topic);

    }
}