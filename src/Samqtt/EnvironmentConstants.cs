namespace Samqtt
{
    internal static class EnvironmentConstants
    {
#if WINDOWS
        public static readonly string UserAppSettingsFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), 
            Constants.AppId.ToLowerInvariant(), 
            Constants.UserAppSettingsFileName);
#else
        public static readonly string UserAppSettingsFile = Path.Combine(
            "/etc/", 
            Constants.AppId.ToLowerInvariant(), 
            Constants.UserAppSettingsFileName);
#endif

    }
}
