using Microsoft.Extensions.DependencyInjection;
using MQTTnet;

namespace Samqtt.Broker.MQTTNet
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMqtt2NetBroker(this IServiceCollection services) => 
            services
            .AddSingleton(_ =>
            {
                var factory = new MqttClientFactory();
                return factory.CreateMqttClient();
            })
            .AddSingleton<IMqttConnectionManager, MqttConnector>()
            .AddSingleton<IMqttPublisher, MqttPublisher>()
            .AddSingleton<IMqttSubscriber, MqttSubscriber>();
    }
}
