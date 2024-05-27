﻿
using System;
using System.Collections.Generic;
using System.Drawing;
using AForge.Video.DirectShow;
using Emgu.CV;

namespace Win2Mqtt.Sensors.HardwareSensors
{
    public static class Camera
    {
        public static List<string> GetDevices()
        {
            List<string> Result = new List<string>();
            FilterInfoCollection VideoCaptureDev = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            foreach (FilterInfo VideoCaptureDevice in VideoCaptureDev)
            {
                Result.Add(VideoCaptureDevice.Name);
            }

            return Result;
        }
        public static bool Save(string FileName)
        {
            try
            {
                VideoCapture capture = new VideoCapture(0);
                Bitmap image = capture.QueryFrame().ToBitmap();
                image.Save(FileName, System.Drawing.Imaging.ImageFormat.Jpeg);

                capture.Dispose();
                image.Dispose();
                return true;
            }
            catch (Exception)
            {

                throw;
            }

        }
    }
}
