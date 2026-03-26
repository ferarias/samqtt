using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Samqtt.SystemSensors.Windows.Sensors
{
    [HomeAssistantSensor(unitOfMeasurement: "%", stateClass: "measurement")]
    public class CpuProcessorTimeSensor(ILogger<CpuProcessorTimeSensor> logger) : SystemSensor<double>, IDisposable
    {
        private readonly PerformanceCounter _cpuCounter = new("Processor", "% Processor Time", "_Total");

        protected override async Task<double> CollectInternalAsync()
        {
            var usage = await Task.Run(() => _cpuCounter.NextValue());
            var rounded = Math.Round(usage, 1);
            logger.LogDebug("Collect {Key}: {Value}%", Metadata.Key, rounded);
            return rounded;
        }

        public void Dispose() => _cpuCounter.Dispose();
    }
}
