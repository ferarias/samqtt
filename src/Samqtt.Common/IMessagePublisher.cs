using System.Threading;
using System.Threading.Tasks;
using Samqtt.SystemActions;
using Samqtt.SystemSensors;

namespace Samqtt
{
    public interface IMessagePublisher
    {
        Task PublishOnlineStatus(CancellationToken cancellationToken = default);

        Task PublishOfflineStatus(CancellationToken cancellationToken = default);

        Task PublishSensorStateDiscoveryMessage(SystemSensorMetadata metadata, CancellationToken cancellationToken = default);

        Task PublishActionStateDiscoveryMessage(SystemActionMetadata metadata, CancellationToken cancellationToken = default);

        Task PublishSensorValue(string stateTopic, string value, CancellationToken cancellationToken = default);
        Task PublishActionStateValue(string stateTopic, string value, CancellationToken cancellationToken = default);

    }
}