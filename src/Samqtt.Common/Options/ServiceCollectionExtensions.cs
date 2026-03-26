using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Samqtt.Options
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSamqttOptions(this IServiceCollection services)
        {
            // ValidateDataAnnotations uses reflection to check [Required] attributes.
            // SamqttOptions and nested types are simple POCOs with no dynamic members —
            // trimming will not remove any properties actually inspected at runtime.
#pragma warning disable IL2026
            services
            .AddOptionsWithValidateOnStart<SamqttOptions>()
                .BindConfiguration(SamqttOptions.SectionName)
                .ValidateDataAnnotations();
#pragma warning restore IL2026

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
