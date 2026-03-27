using Microsoft.Extensions.DependencyInjection;
using Samqtt.SystemSensors.MultiSensors;
using Samqtt.SystemSensors.MultiSensors.Drive;

namespace Samqtt.SystemSensors
{
    /// <summary>
    /// Registers cross-platform sensors (Linux + Windows).
    ///
    /// IMPORTANT: Sensors are NOT discovered automatically. When adding a new cross-platform sensor:
    ///   1. Create the class implementing ISystemSensor (or ISystemMultiSensor) in this project.
    ///   2. Add a corresponding AddSystemSensor&lt;YourSensor&gt;() call below.
    ///   3. Add the sensor key to appsettings.json under "Sensors" or "MultiSensors".
    ///
    /// For Windows-only sensors, register in Samqtt.SystemSensors.Windows/ServiceCollectionExtensions.cs.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemSensors(this IServiceCollection services)
        {
            services
                .AddSystemSensor<Sensors.TimestampSensor>()
                .AddSystemSensor<Sensors.NetworkAvailabilitySensor>();

            services.AddSystemMultiSensor<DriveMultiSensor>()
                .WithChild<DriveFreeSizeSensor>()
                .WithChild<DriveTotalSizeSensor>()
                .WithChild<DrivePercentFreeSizeSensor>()
                .Build();

            // Linux-only sensors
            if (OperatingSystem.IsLinux())
            {
                services.AddSystemSensor<Sensors.CpuProcessorTimeSensor>();
                services.AddSystemSensor<Sensors.FreeMemorySensor>();
            }

            return services;
        }
    }
}
