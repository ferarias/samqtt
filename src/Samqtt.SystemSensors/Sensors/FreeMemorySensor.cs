using Microsoft.Extensions.Logging;

namespace Samqtt.SystemSensors.Sensors
{
    [HomeAssistantSensor(unitOfMeasurement: "B", deviceClass: "memory", stateClass: "measurement")]
    public class FreeMemorySensor(ILogger<FreeMemorySensor> logger) : SystemSensor<double>
    {
        public override string ConfigKey => "FreeMemory";
        public override SensorAttributeInfo GetSensorAttributes() => new()
        {
            UnitOfMeasurement = "B",
            DeviceClass = "memory",
            StateClass = "measurement",
        };

        protected override async Task<double> CollectInternalAsync()
        {
            var value = await ReadMemAvailableBytesAsync();
            logger.LogDebug("Collect {Key}: {Value} B", Metadata.Key, value);
            return value;
        }

        private static async Task<double> ReadMemAvailableBytesAsync()
        {
            // /proc/meminfo contains "MemAvailable:   XXXXXX kB" among other entries
            var text = await File.ReadAllTextAsync("/proc/meminfo").ConfigureAwait(false);
            foreach (var line in text.AsSpan().EnumerateLines())
            {
                if (!line.StartsWith("MemAvailable:", StringComparison.Ordinal))
                    continue;

                // Format: "MemAvailable:   123456 kB"
                var value = line["MemAvailable:".Length..].Trim();
                var spaceIndex = value.IndexOf(' ');
                var kbPart = spaceIndex >= 0 ? value[..spaceIndex] : value;
                return long.Parse(kbPart) * 1024L;
            }

            throw new InvalidOperationException("MemAvailable not found in /proc/meminfo.");
        }
    }
}
