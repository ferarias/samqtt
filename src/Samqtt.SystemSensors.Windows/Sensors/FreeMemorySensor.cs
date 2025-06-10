using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace Samqtt.SystemSensors.Windows.Sensors
{
    [HomeAssistantSensor(unitOfMeasurement: "B", deviceClass: "memory", stateClass: "measurement")]
    public partial class FreeMemorySensor(ILogger<FreeMemorySensor> logger) : SystemSensor<double>
    {
        protected override async Task<double> CollectInternalAsync()

        {
            var value = await GetFreeMemoryAsync();
            logger.LogDebug("Collect {Key}: {Value} B", Metadata.Key, value);
            return value;
        }

        private static Task<double> GetFreeMemoryAsync()
        {
            MEMORYSTATUSEX memoryStatus = new() { dwLength = (uint)Marshal.SizeOf<MEMORYSTATUSEX>() };

            if (!GlobalMemoryStatusEx(ref memoryStatus))
            {
                throw new InvalidOperationException("Unable to query memory status.");
            }

            double freeMemory = memoryStatus.ullAvailPhys;
            return Task.FromResult(freeMemory);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MEMORYSTATUSEX
        {
            public uint dwLength;
            public uint dwMemoryLoad;
            public ulong ullTotalPhys;
            public ulong ullAvailPhys;
            public ulong ullTotalPageFile;
            public ulong ullAvailPageFile;
            public ulong ullTotalVirtual;
            public ulong ullAvailVirtual;
            public ulong ullAvailExtendedVirtual;
        }

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool GlobalMemoryStatusEx(ref MEMORYSTATUSEX lpBuffer);
    }
}