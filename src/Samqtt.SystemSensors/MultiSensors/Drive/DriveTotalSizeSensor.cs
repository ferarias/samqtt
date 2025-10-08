using Microsoft.Extensions.Logging;

namespace Samqtt.SystemSensors.MultiSensors.Drive
{
    [HomeAssistantSensor(unitOfMeasurement: "B", deviceClass: "data_size", stateClass: "measurement")]
    public class DriveTotalSizeSensor(ILogger<DriveTotalSizeSensor> logger) : SystemSensor<long>()
    {
        protected override Task<long> CollectInternalAsync()
        {
            var driveName = OperatingSystem.IsWindows() 
                ? $"{Metadata.InstanceId}:\\"
                : $"{Metadata.InstanceId}";

            var driveInfo = DriveInfo.GetDrives()
                .FirstOrDefault(di => di.Name.Equals(driveName, StringComparison.OrdinalIgnoreCase) ||
                                    di.Name.TrimEnd('/').Equals(driveName, StringComparison.OrdinalIgnoreCase));

            if (driveInfo == null)
            {
                logger.LogWarning("Drive {Key} not found at {Path}", Metadata.InstanceId, driveName);
                return Task.FromResult(0L);
            }
            
            var value = driveInfo.TotalSize;
            logger.LogDebug("Collect {Key}: {Value}", Metadata.Key, value);
            return Task.FromResult(value);
        }
    }
}
