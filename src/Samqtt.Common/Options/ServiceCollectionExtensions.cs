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
                    o.Sensors = new Dictionary<string, SensorOptions>(o.Sensors, StringComparer.OrdinalIgnoreCase);
                    o.MultiSensors = new Dictionary<string, MultiSensorOptions>(o.MultiSensors, StringComparer.OrdinalIgnoreCase);
                    o.Listeners = new Dictionary<string, ListenerOptions>(o.Listeners, StringComparer.OrdinalIgnoreCase);
                });

            return services;
        }
    }
}
