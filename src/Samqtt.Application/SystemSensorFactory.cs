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

            foreach (var sensor in GetEnabledSimpleSensors(allSensors))
                yield return sensor;

            foreach (var sensor in GetEnabledMultiSensors(allSensors, allMultiSensors))
                yield return sensor;
        }

        private IEnumerable<ISystemSensor> GetEnabledSimpleSensors(IEnumerable<ISystemSensor> allSensors)
        {
            foreach (var (sensorName, sensorOptions) in _options.Sensors)
            {
                if (!sensorOptions.Enabled)
                {
                    logger.LogDebug("Sensor {Sensor} is disabled in configuration.", sensorName);
                    continue;
                }

                var sensorInstance = allSensors.FirstOrDefault(a =>
                    a.GetType().Name.Equals(sensorName + "Sensor", StringComparison.OrdinalIgnoreCase));
                if (sensorInstance == null)
                {
                    logger.LogWarning("No sensorInstance ISystemSensor implementation found for key: {Sensor}", sensorName);
                    continue;
                }

                sensorInstance.Metadata = CreateMetadata(
                    sensorInstance.GetType(),
                    SanitizeHelpers.Sanitize(sensorName)
                );

                yield return sensorInstance;
            }
        }

        private IEnumerable<ISystemSensor> GetEnabledMultiSensors(
                    IEnumerable<ISystemSensor> allSensors,
                    IEnumerable<ISystemMultiSensor> allMultiSensors)
        {
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

                foreach (var sensor in GetEnabledMultiSensorChildren(multisensorName, multisensorOptions, multisensorInstance, allSensors))
                    yield return sensor;
            }
        }

        private IEnumerable<ISystemSensor> GetEnabledMultiSensorChildren(
            string multisensorName,
            SystemMultiSensorOptions multisensorOptions,
            ISystemMultiSensor multisensorInstance,
            IEnumerable<ISystemSensor> allSensors)
        {
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
                            "DI could not resolve child sensor `{Sensor}` (child sensor of {Parent}). Check registrations.",
                            childSensorName, multisensorName
                        );
                        continue;
                    }

                    var childTopicName = string.Concat(
                        SanitizeHelpers.Sanitize(multisensorName), '_',
                        SanitizeHelpers.Sanitize(sensorName));

                    sensorInstance.Metadata = CreateMetadata(
                        sensorInstance.GetType(),
                        childTopicName,
                        childId
                    );

                    yield return sensorInstance;
                }
            }
        }

        private SystemSensorMetadata CreateMetadata(Type sensorType, string sensorName, string? instanceId = null)
        {
            var sm = new SystemSensorMetadata
            {
                Key = sensorName,
                Name = sensorType.Name.Replace("Sensor", string.Empty),
                UniqueId = topicProvider.GetUniqueId(sensorName),
                StateTopic = topicProvider.GetSensorStateTopic(sensorName),
                InstanceId = instanceId,
                DiscoveryTopic = topicProvider.GetStandardSensorDiscoveryTopic(sensorName),
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
                sm.DiscoveryTopic = topicProvider.GetBinarySensorDiscoveryTopic(sensorName);
            }
            return sm;
        }
    }
}