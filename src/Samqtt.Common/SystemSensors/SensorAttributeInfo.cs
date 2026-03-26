namespace Samqtt.SystemSensors
{
    /// <summary>
    /// Carries Home Assistant sensor metadata declared on a sensor class.
    /// Replaces runtime <see cref="System.Attribute.GetCustomAttribute"/> calls in the factory.
    /// Each concrete sensor returns an instance of this from <see cref="SystemSensor{T}.GetSensorAttributes"/>.
    /// </summary>
    public readonly struct SensorAttributeInfo
    {
        /// <summary>True when this sensor maps to a binary_sensor in Home Assistant.</summary>
        public bool IsBinary { get; init; }

        // Standard sensor fields
        public string? UnitOfMeasurement { get; init; }
        public string? DeviceClass { get; init; }
        public string? StateClass { get; init; }

        // Binary sensor fields
        public string? PayloadOn { get; init; }
        public string? PayloadOff { get; init; }
    }
}
