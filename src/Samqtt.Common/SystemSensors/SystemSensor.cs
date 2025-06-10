using System.Threading.Tasks;

namespace Samqtt.SystemSensors
{
    public abstract class SystemSensor<T> : ISystemSensor
    {
        public required SystemSensorMetadata Metadata { get; set; }

        /// <summary>
        /// Subclasses override this to return a strongly-typed T.
        /// </summary>
        protected abstract Task<T> CollectInternalAsync();

        /// <summary>
        /// The factory and collector always see an object, so we box the T.
        /// </summary>
        public async Task<object?> CollectAsync()
        {
            var val = await CollectInternalAsync().ConfigureAwait(false);
            return val!;
        }
    }

}
