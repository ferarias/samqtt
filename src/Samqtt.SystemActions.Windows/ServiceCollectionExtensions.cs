using Microsoft.Extensions.DependencyInjection;

namespace Samqtt.SystemActions.Windows
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWindowsSpecificSystemActions(this IServiceCollection services) => services
            .AddSystemActionsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
    }
}
