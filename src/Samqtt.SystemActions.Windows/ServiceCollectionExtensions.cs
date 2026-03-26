using Microsoft.Extensions.DependencyInjection;
using Samqtt.SystemActions;
using Samqtt.SystemActions.Windows.Actions;

namespace Samqtt.SystemActions.Windows
{
    /// <summary>
    /// Registers Windows-only actions.
    ///
    /// IMPORTANT: Actions are NOT discovered automatically. When adding a new Windows-only action:
    ///   1. Create the class implementing ISystemAction in this project.
    ///   2. Add a corresponding AddSystemAction&lt;YourAction&gt;() call below.
    ///   3. Add the action key to appsettings.json under "Actions".
    ///
    /// For cross-platform actions, register in Samqtt.SystemActions/ServiceCollectionExtensions.cs.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddWindowsSpecificSystemActions(this IServiceCollection services) =>
            services
                .AddSystemAction<HibernateAction>()
                .AddSystemAction<RebootAction>()
                .AddSystemAction<ShutdownAction>()
                .AddSystemAction<SuspendAction>();
    }
}
