using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Samqtt.SystemActions
{
    public static class SystemActionsServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemActionsFromAssembly(this IServiceCollection services, Assembly assembly) =>
            services.Scan(scan => scan
            .FromAssemblyDependencies(assembly)
            .AddClasses(classes => classes.AssignableTo<ISystemAction>())
            .AsSelf()
            .AsImplementedInterfaces()
            .WithSingletonLifetime());

        }
}
