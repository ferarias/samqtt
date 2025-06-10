using System.Threading.Tasks;

namespace Samqtt.SystemSensors
{
    public interface ISystemSensor
    {
        /// <summary>
        /// Filled in by the factory before use:
        ///   Key = "FreeMemory", 
        ///   Name = "Free Memory", 
        ///   UniqueId, StateTopic, IsBinary, etc.
        /// </summary>
        SystemSensorMetadata Metadata { get; set; }

        /// <summary>
        /// Return the current sensor value boxed as object.
        /// </summary>
        Task<object?> CollectAsync();
    }

}