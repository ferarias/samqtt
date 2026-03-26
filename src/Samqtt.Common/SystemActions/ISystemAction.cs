using System.Threading;
using System.Threading.Tasks;

namespace Samqtt.SystemActions
{
    public interface ISystemAction
    {
        /// <summary>
        /// The key used in appsettings.json to identify this action (e.g. "Reboot").
        /// </summary>
        string ConfigKey { get; }

        /// <summary>
        /// Whether this action publishes a return value to a state topic.
        /// False for fire-and-forget actions (those returning Unit).
        /// </summary>
        bool ReturnsState { get; }

        SystemActionMetadata Metadata { get; set; }

        Task<object?> HandleAsync(string payload, CancellationToken cancellationToken);
    }
}
