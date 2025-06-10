using System.Reflection;
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
        ILogger<SystemSensorFactory> logger) : ISystemSensorFactory
    {
        private readonly SamqttOptions _options = options.Value;

        public IDictionary<string, ISystemSensor> GetEnabledSensors()
        {
            var sensors = GetSimpleSensors().ToDictionary();

            foreach (var (multisensorName, multisensorInstance) in GetMultiSensors())
            {
                foreach (var (childSensorName, childSensorInstance) in GetMultiSensorChildren(multisensorName, multisensorInstance))
                {
                    sensors.Add(childSensorName, childSensorInstance);
                }
            }

            return sensors;
        }

        private IEnumerable<KeyValuePair<string, ISystemSensor>> GetSimpleSensors()
        {
            var allSensors = serviceProvider.GetServices<ISystemSensor>();
            foreach (var (sensorName, sensorOptions) in _options.Sensors)
            {
                if (!sensorOptions.Enabled)
                {
                    logger.LogInformation("Sensor {Sensor} is disabled in config.", sensorName);
                    continue;
                }

                var sensorType = ResolveSensorType<ISystemSensor>(sensorName, "Sensor");
                if (sensorType is null)
                {
                    logger.LogWarning("No sensor type named `{Type}` exists in AppDomain assemblies", sensorName + "Sensor");
                    continue;
                }

                var sensorInstance = allSensors.FirstOrDefault(sensorType.IsInstanceOfType);
                if (sensorInstance is null)
                {
                    logger.LogWarning("DI could not resolve sensor `{Sensor}` of type {Type}", sensorName, sensorType.FullName);
                    continue;
                }
               
                sensorInstance.Metadata = CreateMetadata(sensorType, sensorName, SanitizeTopicOrDefault(sensorName, sensorOptions.Topic));

                yield return new KeyValuePair<string, ISystemSensor>(sensorName, sensorInstance);
            }
        }

        private IEnumerable<KeyValuePair<string, ISystemMultiSensor>> GetMultiSensors()
        {
            var allMultiSensors = serviceProvider.GetServices<ISystemMultiSensor>();

            foreach (var (multisensorName, multisensorOptions) in _options.MultiSensors)
            {
                if (!multisensorOptions.Enabled)
                {
                    logger.LogInformation("MultiSensor {Sensor} is disabled in config.", multisensorName);
                    continue;
                }

                var multiSensorType = ResolveSensorType<ISystemMultiSensor>(multisensorName, "MultiSensor");
                if (multiSensorType is null)
                {
                    logger.LogWarning("No multi-sensor type named `{Type}` exists in AppDomain assemblies", multisensorName + "MultiSensor");
                    continue;
                }

                var multisensorInstance = allMultiSensors.FirstOrDefault(multiSensorType.IsInstanceOfType);
                if (multisensorInstance is null)
                {
                    logger.LogWarning("DI could not resolve multi-sensor `{Sensor}` of type {Type}", multisensorName, multiSensorType.FullName);
                    continue;
                }

                yield return new KeyValuePair<string, ISystemMultiSensor>(multisensorName, multisensorInstance);
            }
        }

        private IEnumerable<KeyValuePair<string, ISystemSensor>> GetMultiSensorChildren(string multisensorName, ISystemMultiSensor multiSensor)
        {
            var multiSensorOptions = _options.MultiSensors[multisensorName];

            foreach (var (childSensorName, childSensorOptions) in multiSensorOptions.Sensors)
            {
                if (!childSensorOptions.Enabled)
                {
                    logger.LogInformation("Multi-sensor child sensor {Sensor} is disabled in config.", childSensorName);
                    continue;
                }

                var sensorType = ResolveSensorType<ISystemSensor>(childSensorName, "Sensor");
                if (sensorType is null)
                {
                    logger.LogWarning(
                        "No sensor type named `{Type}` exists in AppDomain assemblies (child sensor of {Parent})",
                        childSensorName + "Sensor",
                        multisensorName
                    );
                    continue;
                }

                foreach (var childId in multiSensor.ChildIdentifiers)
                {
                    var sensorName = $"{childSensorName}Sensor_{childId}";
                    var sensorInstance = serviceProvider.GetKeyedService<ISystemSensor>(sensorName);
                    if (sensorInstance is null)
                    {
                        logger.LogWarning(
                            "DI could not resolve child sensor `{Sensor}` of type {Type} (child sensor of {Parent})",
                            childSensorName, sensorType.FullName, multisensorName
                        );
                        continue;
                    }

                    var childTopicName = string.Concat(
                        SanitizeTopicOrDefault(multisensorName, multiSensorOptions.Topic), '_', 
                        SanitizeTopicOrDefault(sensorName, childSensorOptions.Topic));

                    sensorInstance.Metadata = CreateMetadata(sensorType, sensorName, childTopicName, childId);

                    yield return new KeyValuePair<string, ISystemSensor>(sensorName, sensorInstance);
                }
               
            }
        }


        private SystemSensorMetadata CreateMetadata(Type sensorType, string sensorName, string topic, string? instanceId = null)
        {

            var sm = new SystemSensorMetadata
            {
                Key = sensorName,
                Name = sensorType.Name.Replace("Sensor", string.Empty),
                UniqueId = $"{_options.DeviceUniqueId}_{SanitizeHelpers.Sanitize(sensorName)}",
                StateTopic = $"{_options.MqttBaseTopic}/{topic}",
                IsBinary = ReturnsBinaryValue(sensorType),
                InstanceId = instanceId
            };
            if (Attribute.GetCustomAttribute(sensorType, typeof(HomeAssistantSensorAttribute)) is HomeAssistantSensorAttribute haAttr)
            {
                sm.UnitOfMeasurement = haAttr.UnitOfMeasurement;
                sm.DeviceClass = haAttr.DeviceClass;
                sm.StateClass = haAttr.StateClass;
            }
            return sm;
        }

        private static Type? ResolveSensorType<T>(string baseName, string suffix)
        {
            var typeName = baseName + suffix;
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .FirstOrDefault(x =>
                    x.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase)
                    && typeof(T).IsAssignableFrom(x));
        }

        private static string SanitizeTopicOrDefault(string fallback, string? topic) => 
            SanitizeHelpers.Sanitize(string.IsNullOrWhiteSpace(topic) ? fallback : topic);


        private static bool ReturnsBinaryValue(Type t)
        {
            var method = t.GetMethod("CollectInternalAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            if (method?.ReturnType.IsGenericType == true
                && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)
                && method.ReturnType.GetGenericArguments()[0] == typeof(bool))
            {
                return true;
            }

            return false;
        }
    }
}