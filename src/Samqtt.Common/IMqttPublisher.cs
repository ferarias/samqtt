using System.Threading;
using System.Threading.Tasks;

namespace Samqtt
{
    public interface IMqttPublisher
    {
        Task PublishAsync(string topic, string message, bool retain, CancellationToken cancellationToken = default);
        Task PublishAsync((string Topic, string Payload) message, bool retain, CancellationToken cancellationToken = default);
    }
}