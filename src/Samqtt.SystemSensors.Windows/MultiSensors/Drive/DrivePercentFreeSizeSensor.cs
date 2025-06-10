using Microsoft.Extensions.Logging;

namespace Samqtt.SystemSensors.Windows.MultiSensors.Drive
{
    [HomeAssistantSensor(unitOfMeasurement: "%", stateClass: "measurement")]
    public class DrivePercentFreeSizeSensor(ILogger<DrivePercentFreeSizeSensor> logger) : SystemSensor<double>()
    {
        protected override Task<double> CollectInternalAsync()
        {
            var driveName = Metadata.InstanceId + ":\\";
            var driveInfo = DriveInfo.GetDrives().FirstOrDefault(di => di.Name.Equals(driveName, StringComparison.OrdinalIgnoreCase));

            if (driveInfo == null)
            {
                logger.LogWarning("Drive {Key} not found.", driveName);
                return Task.FromResult(0.0); // Return 0% if the drive is not found
            }

            var value = Math.Round((double)driveInfo.TotalFreeSpace / driveInfo.TotalSize * 100, 1);
            logger.LogDebug("Collect {Key}: {Value}", Metadata.Key, value);
            return Task.FromResult(value);
        }
    }
}
