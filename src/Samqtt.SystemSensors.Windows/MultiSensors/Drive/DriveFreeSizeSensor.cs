using Microsoft.Extensions.Logging;

namespace Samqtt.SystemSensors.Windows.MultiSensors.Drive
{
    [HomeAssistantSensor(unitOfMeasurement: "B", deviceClass: "data_size", stateClass: "measurement")]
    public class DriveFreeSizeSensor(ILogger<DriveFreeSizeSensor> logger) : SystemSensor<long>()
    {
        protected override Task<long> CollectInternalAsync()
        {
            var driveName = Metadata.InstanceId + ":\\";
            var driveInfo = DriveInfo.GetDrives().FirstOrDefault(di => di.Name.Equals(driveName, StringComparison.OrdinalIgnoreCase));
            
            if (driveInfo == null)
            {
                logger.LogWarning("Drive {Key} not found.", driveName);
                return Task.FromResult(0L); // Return 0 if the drive is not found
            }

            var value = driveInfo.AvailableFreeSpace;
            logger.LogDebug("Collect {Key}: {Value}", Metadata.Key, value);
            return Task.FromResult(value);
        }
    }
}
