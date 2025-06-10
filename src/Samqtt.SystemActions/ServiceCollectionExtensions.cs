using Microsoft.Extensions.DependencyInjection;

namespace Samqtt.SystemActions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemActions(this IServiceCollection services) => services
            .AddSystemActionsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
    }
}
