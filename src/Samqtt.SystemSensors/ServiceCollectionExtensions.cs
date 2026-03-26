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
        public static IServiceCollection AddSystemSensors(this IServiceCollection services) =>
            services
                // Simple sensors
                .AddSystemSensor<Sensors.TimestampSensor>()
                .AddSystemSensor<Sensors.NetworkAvailabilitySensor>()
                // Multi-sensors (registers parent + keyed child instances per drive/mount)
                .AddSystemMultiSensor<DriveMultiSensor>();
    }
}
