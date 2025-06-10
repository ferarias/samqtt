using System.Threading;
using System.Threading.Tasks;

namespace Samqtt
{
    public interface IMqttConnectionManager
    {
        Task<bool> ConnectAsync(CancellationToken cancellationToken = default);

        Task DisconnectAsync(CancellationToken cancellationToken = default);

        public bool IsConnected { get; }
    }
}