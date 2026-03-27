using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Samqtt.Options
{
    public static class ServiceCollectionExtensions
    {
        // DynamicDependency preserves all members (including data-annotation attributes) on the
        // options types so ValidateDataAnnotations can inspect them at runtime after trimming.
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(SamqttOptions))]
        [DynamicDependency(DynamicallyAccessedMemberTypes.All, typeof(MqttBrokerOptions))]
        [UnconditionalSuppressMessage("Trimming", "IL2026",
            Justification = "DynamicDependency ensures SamqttOptions and MqttBrokerOptions members including data-annotation attributes are preserved.")]
        public static IServiceCollection AddSamqttOptions(this IServiceCollection services)
        {
            services
            .AddOptionsWithValidateOnStart<SamqttOptions>()
                .BindConfiguration(SamqttOptions.SectionName)
                .ValidateDataAnnotations();

            services
                .PostConfigure<SamqttOptions>(o =>
                {
                    o.Sensors = new Dictionary<string, SystemSensorOptions>(o.Sensors, StringComparer.OrdinalIgnoreCase);
                    o.MultiSensors = new Dictionary<string, SystemMultiSensorOptions>(o.MultiSensors, StringComparer.OrdinalIgnoreCase);
                    o.Actions = new Dictionary<string, SystemActionOptions>(o.Actions, StringComparer.OrdinalIgnoreCase);
                });

            return services;
        }
    }
}
