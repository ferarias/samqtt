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
        /// Returns a <see cref="MultiSensorRegistrationBuilder{TMultiSensor}"/> that registers the parent
        /// multi-sensor and lets callers chain <c>.WithChild&lt;T&gt;()</c> for each child sensor type.
        /// Call <c>.Build()</c> to finalize and discover child identifiers (e.g. drive letters) at startup.
        /// </summary>
        public static MultiSensorRegistrationBuilder<TMultiSensor> AddSystemMultiSensor<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMultiSensor>(
            this IServiceCollection services)
            where TMultiSensor : class, ISystemMultiSensor
        {
            return new MultiSensorRegistrationBuilder<TMultiSensor>(services);
        }
    }
}
