using Microsoft.Extensions.DependencyInjection;
using Samqtt.SystemActions.Actions;

namespace Samqtt.SystemActions
{
    /// <summary>
    /// Registers cross-platform actions (Linux + Windows).
    ///
    /// IMPORTANT: Actions are NOT discovered automatically. When adding a new cross-platform action:
    ///   1. Create the class implementing ISystemAction in this project.
    ///   2. Add a corresponding AddSystemAction&lt;YourAction&gt;() call below.
    ///   3. Add the action key to appsettings.json under "Actions".
    ///
    /// For Windows-only actions, register in Samqtt.SystemActions.Windows/ServiceCollectionExtensions.cs.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSystemActions(this IServiceCollection services)
        {
            services
                .AddSystemAction<GetProcessAction>()
                .AddSystemAction<GetProcessesAction>()
                .AddSystemAction<KillProcessAction>()
                .AddSystemAction<StartProcessAction>();

            if (OperatingSystem.IsLinux())
            {
                services.AddSystemAction<SuspendAction>();
                services.AddSystemAction<ShutdownAction>();
                services.AddSystemAction<RebootAction>();
                services.AddSystemAction<HibernateAction>();
                services.AddSystemAction<SendNotificationAction>();
            }

            return services;
        }
    }
}
