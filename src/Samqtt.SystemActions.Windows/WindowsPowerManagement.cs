using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Samqtt.SystemActions.Windows
{
    public static partial class WindowsPowerManagement
    {
        [DllImport("Powrprof.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetSuspendState(bool hibernate, bool forceCritical, bool disableWakeEvent);

        public static bool HibernateSystem() => SetSuspendState(true, false, false);
        public static bool SuspendSystem() => SetSuspendState(false, false, false);

        public static void Shutdown(int delay)
        {
            Process.Start("shutdown.exe", $"-s -t {delay}");
        }
        public static void Reboot(int delay)
        {
            Process.Start("shutdown.exe", $"-r -t {delay}");
        }
    }
}
