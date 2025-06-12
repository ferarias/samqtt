using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Samqtt.SystemActions
{
    public static class SystemActionsServiceCollectionExtensions
    {
        /// <summary>
        /// Add System Actions to the service collection
        /// </summary>
        /// <param name="services"></param>
        /// <param name="assembly"></param>
        /// <returns></returns>
        public static IServiceCollection AddSystemActionsFromAssembly(this IServiceCollection services, Assembly assembly) =>
            services
            .Scan(scan => scan
                .FromAssemblyDependencies(assembly)
                .AddClasses(classes => classes.AssignableTo<ISystemAction>(), publicOnly: true)
                .AsSelf()
                .As<ISystemAction>()
                .WithSingletonLifetime());

        }
}
