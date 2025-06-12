using System;

namespace Samqtt.SystemSensors
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class HomeAssistantBinarySensorAttribute(string? payloadOn = "on", string? payloadOff = "off", string? deviceClass = null) : Attribute
    {
        public string? PayloadOn { get; } = payloadOn;
        public string? DeviceClass { get; } = deviceClass;
        public string? PayLoadOff { get; } = payloadOff;
    }
}
