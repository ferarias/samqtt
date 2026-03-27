using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Samqtt.SystemSensors.Windows.Sensors
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

        private long _prevIdle;
        private long _prevKernel;
        private long _prevUser;
        private bool _initialized;

        protected override Task<double> CollectInternalAsync()
        {
            GetSystemTimes(out var idleTime, out var kernelTime, out var userTime);

            long idle   = idleTime.ToLong();
            long kernel = kernelTime.ToLong();
            long user   = userTime.ToLong();

            if (!_initialized)
            {
                _prevIdle   = idle;
                _prevKernel = kernel;
                _prevUser   = user;
                _initialized = true;
                return Task.FromResult(0.0);
            }

            long deltaIdle   = idle   - _prevIdle;
            long deltaKernel = kernel - _prevKernel;
            long deltaUser   = user   - _prevUser;

            _prevIdle   = idle;
            _prevKernel = kernel;
            _prevUser   = user;

            // kernel time includes idle time on Windows
            long deltaTotal = deltaKernel + deltaUser;

            if (deltaTotal == 0)
                return Task.FromResult(0.0);

            var usage = Math.Round((double)(deltaTotal - deltaIdle) / deltaTotal * 100.0, 1);
            logger.LogDebug("Collect {Key}: {Value}%", Metadata.Key, usage);
            return Task.FromResult(usage);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool GetSystemTimes(
            out FILETIME lpIdleTime,
            out FILETIME lpKernelTime,
            out FILETIME lpUserTime);

        [StructLayout(LayoutKind.Sequential)]
        private struct FILETIME
        {
            public uint dwLowDateTime;
            public uint dwHighDateTime;
            public long ToLong() => ((long)dwHighDateTime << 32) | dwLowDateTime;
        }
    }
}
