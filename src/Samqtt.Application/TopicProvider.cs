using Microsoft.Extensions.Options;

using Samqtt.Common;
using Samqtt.Options;

namespace Samqtt.Application
{
    public class TopicProvider(IOptionsMonitor<SamqttOptions> options) : ITopicProvider
    {
        /// <summary>
        /// eg: samqtt
        /// </summary>
        private static readonly string _appIdPrefix = Constants.AppId.ToLowerInvariant();

        /// <summary>
        /// eg: lenovo_laptop
        /// </summary>
        private readonly string _deviceIdentifier = options.CurrentValue?.DeviceIdentifier
                ?? throw new ArgumentNullException(nameof(options), "SamqttOptions.DeviceIdentifier cannot be null.");

        /// <summary>
        /// eg: samqtt_lenovo_laptop
        /// </summary>
        private string AppUniqueIdPrefix => $"{_appIdPrefix}_{_deviceIdentifier}";

        /// <summary>
        /// eg: samqtt/lenovo_laptop/status
        /// </summary>
        public string StatusTopic => $"{_appIdPrefix}/{_deviceIdentifier}/status";

        /// <summary>
        /// eg: samqtt/lenovo_laptop/#
        /// </summary>
        public string UnsubscribeTopic => $"{_appIdPrefix}/{_deviceIdentifier}/#";

        /// <summary>
        /// eg: samqtt_lenovo_laptop_cpu_temperature
        /// </summary>
        /// <param name="name">Name of the instance/sensor</param>
        /// <returns></returns>
        public string GetUniqueId(string name) => $"{AppUniqueIdPrefix}_{SanitizeHelpers.Sanitize(name)}";

        /// <summary>
        /// eg: samqtt/system_sensor/lenovo_laptop/cpu_temperature/state
        /// </summary>
        /// <param name="sensorName">Sensor name</param>
        /// <returns></returns>
        public string GetSensorStateTopic(string sensorName) => $"{_appIdPrefix}/system_sensor/{_deviceIdentifier}/{SanitizeHelpers.Sanitize(sensorName)}/state";

        /// <summary>
        /// eg: samqtt/system_action/lenovo_laptop/cpu_temperature/state
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <returns></returns>
        public string GetActionStateTopic(string actionName) => $"{_appIdPrefix}/system_action/{_deviceIdentifier}/{SanitizeHelpers.Sanitize(actionName)}/state";
        public string GetActionJsonAttributesTopic(string actionName) => $"{_appIdPrefix}/system_action/{_deviceIdentifier}/{SanitizeHelpers.Sanitize(actionName)}/attributes";

        /// <summary>
        /// eg: samqtt/system_action/lenovo_laptop/cpu_temperature/request
        /// </summary>
        /// <param name="actionName"></param>
        /// <returns></returns>
        public string GetActionCommandTopic(string actionName) => $"{_appIdPrefix}/system_action/{_deviceIdentifier}/{SanitizeHelpers.Sanitize(actionName)}/request";

        /// <summary>
        /// eg: homeassistant/sensor|binary_sensor/samqtt_lenovo_laptop_cpu_temperature/config
        /// </summary>
        /// <param name="sensorName">Sensor name</param>
        /// <param name="isBinary">Is a binary sensor</param>
        /// <returns></returns>
        public string GetSensorDiscoveryTopic(string sensorName, bool isBinary) => isBinary 
            ? $"{Constants.HomeAssistantTopic}/binary_sensor/{AppUniqueIdPrefix}_{SanitizeHelpers.Sanitize(sensorName)}/config"
            : $"{Constants.HomeAssistantTopic}/sensor/{AppUniqueIdPrefix}_{SanitizeHelpers.Sanitize(sensorName)}/config";

        /// <summary>
        /// eg: homeassistant/sensor/samqtt_lenovo_laptop_cpu_temperature/config
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <returns></returns>
        public string GetActionDiscoveryTopic(string actionName) => $"{Constants.HomeAssistantTopic}/sensor/{AppUniqueIdPrefix}_{SanitizeHelpers.Sanitize(actionName)}/config";

    }
}

    
