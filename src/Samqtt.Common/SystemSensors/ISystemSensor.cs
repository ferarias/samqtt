using System.Threading.Tasks;

namespace Samqtt.SystemSensors
{
    public interface ISystemSensor
    {
        /// <summary>
        /// The key used in appsettings.json to identify this sensor (e.g. "FreeMemory").
        /// </summary>
        string ConfigKey { get; }

        /// <summary>
        /// Filled in by the factory before use.
        /// </summary>
        SystemSensorMetadata Metadata { get; set; }

        /// <summary>
        /// Returns Home Assistant sensor attribute values without reflection.
        /// </summary>
        SensorAttributeInfo GetSensorAttributes();

        /// <summary>
        /// Return the current sensor value boxed as object.
        /// </summary>
        Task<object?> CollectAsync();
    }
}
