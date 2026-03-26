using System.Threading;
using System.Threading.Tasks;

namespace Samqtt
{
    public interface IMqttPublisher
    {
        /// <summary>
        /// Publishes a message to the given topic.
        /// </summary>
        /// <param name="atLeastOnce">
        /// When <c>true</c>, uses QoS 1 (AtLeastOnce) — suitable for control-plane messages such as
        /// discovery payloads and LWT status. When <c>false</c> (default), uses QoS 0 (AtMostOnce)
        /// which is appropriate for sensor telemetry where occasional loss is acceptable.
        /// </param>
        Task PublishAsync(string topic, string message, bool retain, bool atLeastOnce = false, CancellationToken cancellationToken = default);
    }
}