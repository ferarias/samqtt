using Microsoft.Extensions.Logging;
using System.Net.NetworkInformation;

namespace Samqtt.SystemSensors.Sensors
{
    [HomeAssistantSensor(unitOfMeasurement: "availability", deviceClass: "connectivity")]
    public class NetworkAvailabilitySensor(ILogger<NetworkAvailabilitySensor> logger) : SystemSensor<bool>
    {
        protected override Task<bool> CollectInternalAsync()
        {
            var value = NetworkInterface.GetIsNetworkAvailable();
            logger.LogDebug("Collect {Key}: {Value}", Metadata.Key, value);
            return Task.FromResult(value);
        }
    }
}
