namespace Samqtt.SystemSensors.MultiSensors
{
    public class DriveMultiSensor() : SystemMultiSensorBase
    {
        public override IEnumerable<string> ChildIdentifiers => GetDriveLetters();

        private static IEnumerable<string> GetDriveLetters()
        {
            var drives = DriveInfo.GetDrives().Where(di =>
                di.IsReady &&
                di.DriveType != DriveType.Network &&
                di.DriveType != DriveType.Ram &&
                di.DriveFormat != "overlay");
            return drives.Where(di =>
                        (
                            // Windows drives
                            di.Name.EndsWith(":\\") ||
                            // Linux/WSL mounted drives
                            di.Name.StartsWith("/mnt/") ||
                            di.Name.StartsWith("/media/")
                        ))
            .Select(di => di.Name.Replace(":\\", ""));
        }
    }
}
