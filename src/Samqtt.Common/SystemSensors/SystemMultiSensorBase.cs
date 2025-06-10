using System.Collections.Generic;

namespace Samqtt.SystemSensors
{
    public abstract class SystemMultiSensorBase : ISystemMultiSensor
    {
        public virtual IEnumerable<string> ChildIdentifiers => [];
    }
}
