using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;

namespace Samqtt.SystemActions
{
    public static class SystemActionsServiceCollectionExtensions
    {
        /// <summary>
        /// Registers a single <see cref="ISystemAction"/> implementation as both its concrete type
        /// and as <see cref="ISystemAction"/> with singleton lifetime.
        /// </summary>
        public static IServiceCollection AddSystemAction<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] TImplementation>(this IServiceCollection services)
            where TImplementation : class, ISystemAction
        {
            services.AddSingleton<TImplementation>();
            services.AddSingleton<ISystemAction, TImplementation>(sp => sp.GetRequiredService<TImplementation>());
            return services;
        }
    }
}
