using System.Collections.Generic;

namespace Samqtt.SystemSensors
{
    public abstract class SystemMultiSensorBase : ISystemMultiSensor
    {
        public abstract string ConfigKey { get; }
        public virtual IEnumerable<string> ChildIdentifiers => [];
    }
}
