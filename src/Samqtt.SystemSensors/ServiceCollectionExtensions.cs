using Microsoft.Extensions.DependencyInjection;

namespace Samqtt.SystemSensors
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add system sensors to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSystemSensors(this IServiceCollection services) => 
            services.AddSystemSensorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
        
    }
}
