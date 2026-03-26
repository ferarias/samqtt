using System.Threading;
using System.Threading.Tasks;

namespace Samqtt.SystemActions
{
    public abstract class SystemAction<T> : ISystemAction
    {
        public required SystemActionMetadata Metadata { get; set; }

        /// <summary>
        /// The key used in appsettings.json to identify this action (e.g. "Reboot").
        /// Each concrete action must override this with its exact config key.
        /// This replaces the previous runtime reflection over GetType().Name.
        /// </summary>
        public abstract string ConfigKey { get; }

        /// <summary>
        /// Whether this action publishes a return value to a state topic.
        /// Defaults to true unless T is <see cref="Unit"/> (fire-and-forget actions).
        /// Replaces the previous <c>actionInstance is SystemAction&lt;Unit&gt;</c> type-check.
        /// </summary>
        public virtual bool ReturnsState => typeof(T) != typeof(Unit);

        public async Task<object?> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            return await HandleCoreAsync(payload, cancellationToken);
        }

        public abstract Task<T> HandleCoreAsync(string payload, CancellationToken cancellationToken);
    }
}
