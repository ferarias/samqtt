
namespace Samqtt.SystemSensors
{
    public class SystemSensorMetadata : MetadataBase
    {
        /// <summary>
        /// Used in multi-sensor contexts to differentiate between "child" instances.
        /// </summary>
        public string? InstanceId { get; init; }

        public bool IsBinary { get; set; }

        public string? UnitOfMeasurement { get; set; }

        public string? DeviceClass { get; set; }

        public string? StateClass { get; set; }

        public string? PayloadOn { get; set; } = "on";

        public string? PayloadOff { get; set; } = "off";
    }
}
