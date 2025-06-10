using System.Threading;
using System.Threading.Tasks;

namespace Samqtt.SystemActions
{
    public abstract class SystemAction<T> : ISystemAction
    {
        public required SystemActionMetadata Metadata { get; set; }

        public abstract Task<T> HandleCoreAsync(string payload, CancellationToken cancellationToken);

        public async Task<object?> HandleAsync(string payload, CancellationToken cancellationToken)
        {
            return await HandleCoreAsync(payload, cancellationToken);
        }
    }

}
