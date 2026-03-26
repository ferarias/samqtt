using Microsoft.Extensions.DependencyInjection;
using Samqtt.SystemSensors;

namespace Samqtt.SystemSensors.Windows
{
    /// <summary>
    /// Registers Windows-only sensors.
    ///
    /// IMPORTANT: Sensors are NOT discovered automatically. When adding a new Windows-only sensor:
    ///   1. Create the class implementing ISystemSensor in this project.
    ///   2. Add a corresponding AddSystemSensor&lt;YourSensor&gt;() call below.
    ///   3. Add the sensor key to appsettings.json under "Sensors".
    ///
    /// For cross-platform sensors, register in Samqtt.SystemSensors/ServiceCollectionExtensions.cs.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWindowsSpecificSystemSensors(this IServiceCollection services) =>
            services
                .AddSystemSensor<Sensors.CpuProcessorTimeSensor>()
                .AddSystemSensor<Sensors.FreeMemorySensor>();
    }
}
