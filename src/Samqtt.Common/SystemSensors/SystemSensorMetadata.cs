using System;

namespace Samqtt.SystemSensors
{
    public class SystemSensorMetadata
    {
        public string Key { get; init; } = default!;

        public string? InstanceId { get; init; }

        public string Name { get; set; } = default!;

        public string? UniqueId { get; set; }

        public string? StateTopic { get; set; }

        public bool IsBinary { get; set; }

        public string? UnitOfMeasurement { get; set; }

        public string? DeviceClass { get; set; }

        public string? StateClass { get; set; }
    }
}
