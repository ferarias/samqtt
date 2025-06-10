using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace Samqtt.Options
{
    public static class ServiceCollectionExtensions
    {
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
