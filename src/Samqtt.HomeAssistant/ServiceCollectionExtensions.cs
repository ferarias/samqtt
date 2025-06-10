using Microsoft.Extensions.DependencyInjection;
using Samqtt.SystemSensors;

namespace Samqtt.HomeAssistant
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHomeAssistant(this IServiceCollection services) =>
            services
            .AddSingleton<ISystemSensorValueFormatter, HomeAssistantSensorValueFormatter>()
            .AddSingleton<IMessagePublisher, HomeAssistantPublisher>();
    }
}
