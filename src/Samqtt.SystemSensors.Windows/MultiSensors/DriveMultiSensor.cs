namespace Samqtt.SystemSensors.Windows.MultiSensors
{
    public class DriveMultiSensor() : SystemMultiSensorBase
    {
        public override IEnumerable<string> ChildIdentifiers => GetDriveLetters();

        private static IEnumerable<string> GetDriveLetters() => DriveInfo.GetDrives()
            .Where(di => di.IsReady && di.DriveType != DriveType.Network)
            .Select(di => di.Name.Replace(":\\", ""));
    }

}
