using Samqtt.SystemActions;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Samqtt
{
    public interface IMqttSubscriber
    {
        Task<bool> SubscribeAsync(SystemActionMetadata metadata, Func<string, CancellationToken, Task<object?>> handler, CancellationToken cancellationToken = default);
    }
}