using System;

namespace Samqtt.SystemSensors
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class HomeAssistantSensorAttribute(string? unitOfMeasurement = null, string? deviceClass = null, string? stateClass = null) : Attribute
    {
        public string? UnitOfMeasurement { get; } = unitOfMeasurement;
        public string? DeviceClass { get; } = deviceClass;
        public string? StateClass { get; } = stateClass;
    }

}
