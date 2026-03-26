using Microsoft.Extensions.Logging;

namespace Samqtt.SystemSensors.Sensors
{
    [HomeAssistantSensor(deviceClass: "timestamp")]
    public class TimestampSensor(ILogger<TimestampSensor> logger) : SystemSensor<DateTime>
    {
        public override string ConfigKey => "Timestamp";
        public override SensorAttributeInfo GetSensorAttributes() => new() { DeviceClass = "timestamp" };

        protected override Task<DateTime> CollectInternalAsync()

        {
            var value = DateTime.UtcNow;
            logger.LogDebug("Collect {Key}: {Value}", Metadata.Key, value);
            return Task.FromResult(value);
        }
    }
}
