using System.Collections.Generic;

namespace Samqtt.SystemSensors
{
    public interface ISystemSensorFactory
    {
        public IDictionary<string, ISystemSensor> GetEnabledSensors();
    }
}
