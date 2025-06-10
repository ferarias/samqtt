using Microsoft.Extensions.DependencyInjection;

namespace Samqtt.SystemSensors.Windows
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add system sensors to the service collection.
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddWindowsSpecificSystemSensors(this IServiceCollection services) => 
            services.AddSystemSensorsFromAssembly(typeof(ServiceCollectionExtensions).Assembly);
    }
}
