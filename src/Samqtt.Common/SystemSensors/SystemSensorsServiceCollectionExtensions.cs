using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Samqtt.SystemSensors
{
    public static class SystemSensorsServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a single <see cref="ISystemSensor"/> implementation as both its concrete type
        /// and as <see cref="ISystemSensor"/> with singleton lifetime.
        /// </summary>
        public static IServiceCollection AddSystemSensor<TImplementation>(this IServiceCollection services)
            where TImplementation : class, ISystemSensor
        {
            services.AddSingleton<TImplementation>();
            services.AddSingleton<ISystemSensor, TImplementation>(sp => sp.GetRequiredService<TImplementation>());
            return services;
        }

        /// <summary>
        /// Registers a single <see cref="ISystemMultiSensor"/> implementation as <see cref="ISystemMultiSensor"/>
        /// with singleton lifetime, then registers all keyed child sensor instances for each
        /// drive/mount discovered at startup.
        /// </summary>
        public static IServiceCollection AddSystemMultiSensor<TMultiSensor>(this IServiceCollection services)
            where TMultiSensor : class, ISystemMultiSensor
        {
            services.AddSingleton<ISystemMultiSensor, TMultiSensor>();

            // Temporarily build a provider to resolve the registered multi-sensor and discover
            // its child identifiers (e.g. drive letters / mount points) at startup time.
            using var provider = services.BuildServiceProvider();
            var multiSensors = provider.GetServices<ISystemMultiSensor>();

            foreach (var sensor in multiSensors)
            {
                services.AddMultiSensorChildSensors(sensor);
            }

            return services;
        }

        private static IServiceCollection AddMultiSensorChildSensors(this IServiceCollection services, ISystemMultiSensor sensor)
        {
            // Child sensor types are discovered by convention: their class name starts with the
            // multi-sensor base name (e.g. "Drive" from "DriveMultiSensor").
            // Example: DriveMultiSensor → DriveFreeSizeSensor, DriveTotalSizeSensor, ...
            // To add a new child sensor type, create a class in the same assembly whose name
            // starts with the parent's base name and implements ISystemSensor, then register the
            // parent multi-sensor via AddSystemMultiSensor<T>() — child types are picked up
            // automatically through this convention.
            var type = sensor.GetType();
            var sensorBaseName = type.Name.Replace("MultiSensor", string.Empty);
            var sensorTypes = type.Assembly
                .GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract &&
                    typeof(ISystemSensor).IsAssignableFrom(t) &&
                    t.Name.StartsWith(sensorBaseName, StringComparison.OrdinalIgnoreCase));

            foreach (var id in sensor.ChildIdentifiers)
            {
                foreach (var sensorType in sensorTypes)
                {
                    var key = $"{sensorType.Name}_{id}";
                    services.AddKeyedSingleton(typeof(ISystemSensor), key, sensorType);
                }
            }
            return services;
        }
    }
}
