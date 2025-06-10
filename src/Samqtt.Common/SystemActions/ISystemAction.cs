using System.Threading;
using System.Threading.Tasks;

namespace Samqtt.SystemActions
{
    public interface ISystemAction
    {
        SystemActionMetadata Metadata { get; set; }

        Task<object?> HandleAsync(string payload, CancellationToken cancellationToken);
    }

}
