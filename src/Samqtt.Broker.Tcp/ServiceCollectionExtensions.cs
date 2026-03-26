using Microsoft.Extensions.DependencyInjection;

namespace Samqtt.Broker.Tcp;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTcpMqttBroker(this IServiceCollection services) =>
        services
            .AddSingleton<MqttTcpClient>()
            .AddSingleton<IMqttConnectionManager, MqttConnector>()
            .AddSingleton<IMqttPublisher, MqttPublisher>()
            .AddSingleton<IMqttSubscriber, MqttSubscriber>();
}
