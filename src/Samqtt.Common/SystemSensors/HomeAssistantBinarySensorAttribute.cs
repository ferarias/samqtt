using System;

namespace Samqtt.SystemSensors
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class HomeAssistantBinarySensorAttribute(string? deviceClass = null, string ? payloadOn = "on", string? payloadOff = "off") : Attribute
    {
        public string? DeviceClass { get; } = deviceClass;
        public string? PayloadOn { get; } = payloadOn;
        public string? PayloadOff { get; } = payloadOff;
    }
}
