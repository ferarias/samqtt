﻿using System.Runtime.InteropServices;

namespace Win2Mqtt.Infra.HardwareSensors
{
    internal struct Lastinputinfo
    {
        public uint CbSize;

        public uint DwTime;
    }

    /// <summary>
    /// Helps to find the idle time, (in milliseconds) spent since the last user input
    /// </summary>
    public static class IdleTimeFinder
    {
        private class NativeMethods
        {
            [DllImport("User32.dll")]
            public static extern bool GetLastInputInfo(ref Lastinputinfo plii);
        }


        public static TimeSpan GetIdleTime()
        {
            var lastInPut = new Lastinputinfo();
            lastInPut.CbSize = (uint)Marshal.SizeOf(lastInPut);
            NativeMethods.GetLastInputInfo(ref lastInPut);

            return TimeSpan.FromMilliseconds((uint)Environment.TickCount - lastInPut.DwTime);
        }
    }
}
