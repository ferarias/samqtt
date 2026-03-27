using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Samqtt.SystemSensors
{
    public sealed class MultiSensorRegistrationBuilder<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TMultiSensor>
        where TMultiSensor : class, ISystemMultiSensor
    {
        private readonly IServiceCollection _services;
        private readonly List<System.Action<IServiceCollection>> _templateRegistrations = [];
        private readonly List<System.Action<IServiceCollection, string>> _keyedRegistrations = [];

        internal MultiSensorRegistrationBuilder(IServiceCollection services)
        {
            _services = services;
            services.AddSingleton<ISystemMultiSensor, TMultiSensor>();
        }

        public MultiSensorRegistrationBuilder<TMultiSensor> WithChild<
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TChild>()
            where TChild : class, ISystemSensor
        {
            var typeName = typeof(TChild).Name; // safe in AOT — type names are always preserved

            _templateRegistrations.Add(services =>
            {
                services.AddSingleton<TChild>();
                services.AddSingleton<ISystemSensor>(sp => sp.GetRequiredService<TChild>());
            });

            _keyedRegistrations.Add((services, id) =>
            {
                var key = $"{typeName}_{id}";
                services.AddKeyedSingleton<ISystemSensor, TChild>(key);
            });

            return this;
        }

        public IServiceCollection Build()
        {
            // Register non-keyed template instances (used by SystemSensorFactory for config validation)
            foreach (var reg in _templateRegistrations)
                reg(_services);

            // Build a temporary provider to discover child identifiers (drive letters / mount points)
            using var provider = _services.BuildServiceProvider();
            foreach (var sensor in provider.GetServices<ISystemMultiSensor>())
            {
                foreach (var id in sensor.ChildIdentifiers)
                {
                    foreach (var reg in _keyedRegistrations)
                        reg(_services, id);
                }
            }

            return _services;
        }
    }
}
