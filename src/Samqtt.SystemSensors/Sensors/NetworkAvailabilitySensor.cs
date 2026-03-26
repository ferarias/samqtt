using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;

namespace Samqtt.SystemSensors.Sensors
{
    [HomeAssistantBinarySensor(deviceClass: "connectivity")]
    public class NetworkAvailabilitySensor(ILogger<NetworkAvailabilitySensor> logger) : SystemSensor<bool>
    {
        public override string ConfigKey => "NetworkAvailability";
        public override SensorAttributeInfo GetSensorAttributes() => new()
        {
            IsBinary = true,
            DeviceClass = "connectivity",
            PayloadOn = "on",
            PayloadOff = "off",
        };

        protected override Task<bool> CollectInternalAsync()
        {
            var value = NetworkInterface.GetIsNetworkAvailable();
            logger.LogDebug("Collect {Key}: {Value}", Metadata.Key, value);
            return Task.FromResult(value);
        }
    }
}
