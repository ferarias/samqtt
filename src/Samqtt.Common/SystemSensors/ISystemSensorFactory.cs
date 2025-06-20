using System.Collections.Generic;

namespace Samqtt.SystemSensors
{
    public interface ISystemSensorFactory
    {
        public IEnumerable<ISystemSensor> GetEnabledSensors();
    }
}
