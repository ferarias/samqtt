using Microsoft.Extensions.DependencyInjection;
using Samqtt.SystemActions;
using Samqtt.SystemSensors;

namespace Samqtt.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSamqttApplication(this IServiceCollection services) =>
            services
            .AddSingleton<ISystemSensorFactory, SystemSensorFactory>()
            .AddSingleton<ISystemActionFactory, SystemActionFactory>();
    }
}
