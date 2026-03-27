using Microsoft.Extensions.Logging;

namespace Samqtt.SystemSensors.Sensors
{
    [HomeAssistantSensor(unitOfMeasurement: "%", stateClass: "measurement")]
    public class CpuProcessorTimeSensor(ILogger<CpuProcessorTimeSensor> logger) : SystemSensor<double>
    {
        public override string ConfigKey => "CpuProcessorTime";
        public override SensorAttributeInfo GetSensorAttributes() => new()
        {
            UnitOfMeasurement = "%",
            StateClass = "measurement",
        };

        private long _prevTotal;
        private long _prevIdle;
        private bool _initialized;

        protected override async Task<double> CollectInternalAsync()
        {
            var (total, idle) = await ReadCpuTimesAsync();

            if (!_initialized)
            {
                _prevTotal = total;
                _prevIdle = idle;
                _initialized = true;
                return 0.0;
            }

            var deltaTotal = total - _prevTotal;
            var deltaIdle = idle - _prevIdle;

            _prevTotal = total;
            _prevIdle = idle;

            if (deltaTotal == 0)
                return 0.0;

            var usage = Math.Round((double)(deltaTotal - deltaIdle) / deltaTotal * 100.0, 1);
            logger.LogDebug("Collect {Key}: {Value}%", Metadata.Key, usage);
            return usage;
        }

        private static async Task<(long total, long idle)> ReadCpuTimesAsync()
        {
            // /proc/stat first line: "cpu  user nice system idle iowait irq softirq steal ..."
            var text = await File.ReadAllTextAsync("/proc/stat").ConfigureAwait(false);
            var newline = text.IndexOf('\n');
            var firstLine = newline >= 0 ? text[..newline] : text;
            var parts = firstLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            // parts[0] = "cpu", parts[1..8] = user nice system idle iowait irq softirq steal

            long user    = long.Parse(parts[1]);
            long nice    = long.Parse(parts[2]);
            long system  = long.Parse(parts[3]);
            long idle    = long.Parse(parts[4]);
            long iowait  = long.Parse(parts[5]);
            long irq     = long.Parse(parts[6]);
            long softirq = long.Parse(parts[7]);
            long steal   = parts.Length > 8 ? long.Parse(parts[8]) : 0;

            long totalIdle = idle + iowait;
            long total     = user + nice + system + idle + iowait + irq + softirq + steal;

            return (total, totalIdle);
        }
    }
}
