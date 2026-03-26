using System.Threading.Tasks;

namespace Samqtt.SystemSensors
{
    public abstract class SystemSensor<T> : ISystemSensor
    {
        public required SystemSensorMetadata Metadata { get; set; }

        /// <summary>
        /// The key used in appsettings.json to identify this sensor (e.g. "FreeMemory").
        /// Each concrete sensor must override this with its exact config key.
        /// This replaces the previous runtime reflection over GetType().Name.
        /// </summary>
        public abstract string ConfigKey { get; }

        /// <summary>
        /// Returns Home Assistant sensor attribute values declared on this sensor.
        /// Override in subclasses that carry <see cref="HomeAssistantSensorAttribute"/> or
        /// <see cref="HomeAssistantBinarySensorAttribute"/> to supply the values without reflection.
        /// The default returns a plain sensor with no HA metadata.
        /// </summary>
        public virtual SensorAttributeInfo GetSensorAttributes() => default;

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
