using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Samqtt.SystemSensors
{
    public static class SystemSensorsServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a single <see cref="ISystemSensor"/> implementation as both its concrete type
        /// and as <see cref="ISystemSensor"/> with singleton lifetime.
        /// </summary>
        public static IServiceCollection AddSystemSensor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
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
        /// <param name="childSensorTypes">
        /// The concrete child sensor types to register per identifier (e.g. per drive letter).
        /// Must be provided explicitly — child types are no longer discovered by reflection.
        /// </param>
        public static IServiceCollection AddSystemMultiSensor<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMultiSensor>(
            this IServiceCollection services,
            params Type[] childSensorTypes)
            where TMultiSensor : class, ISystemMultiSensor
        {
            services.AddSingleton<ISystemMultiSensor, TMultiSensor>();

            // Temporarily build a provider to resolve the registered multi-sensor and discover
            // its child identifiers (e.g. drive letters / mount points) at startup time.
            using var provider = services.BuildServiceProvider();
            var multiSensors = provider.GetServices<ISystemMultiSensor>();

            foreach (var sensor in multiSensors)
            {
                services.AddMultiSensorChildSensors(sensor, childSensorTypes);
            }

            return services;
        }

        private static IServiceCollection AddMultiSensorChildSensors(
            this IServiceCollection services,
            ISystemMultiSensor sensor,
            Type[] childSensorTypes)
        {
            // Register each child type once as a non-keyed ISystemSensor.
            // SystemSensorFactory.GetEnabledMultiSensorChildren() first searches GetServices<ISystemSensor>()
            // (non-keyed only) to validate that a config key is backed by a known implementation.
            // Without this, the template check always returns null and the per-drive keyed instances
            // are never reached.
            foreach (var sensorType in childSensorTypes)
            {
#pragma warning disable IL2072
                services.AddSingleton(sensorType);
                services.AddSingleton(typeof(ISystemSensor), sp => sp.GetRequiredService(sensorType));
#pragma warning restore IL2072
            }

            foreach (var id in sensor.ChildIdentifiers)
            {
                foreach (var sensorType in childSensorTypes)
                {
                    var key = $"{sensorType.Name}_{id}";
                    // Callers are required to pass concrete types with public constructors.
                    // The DynamicallyAccessedMembers annotation cannot flow through Type[],
                    // so we suppress here — all call sites pass typeof(ConcreteClass) literals.
#pragma warning disable IL2072
                    services.AddKeyedSingleton(typeof(ISystemSensor), key, sensorType);
#pragma warning restore IL2072
                }
            }
            return services;
        }
    }
}
