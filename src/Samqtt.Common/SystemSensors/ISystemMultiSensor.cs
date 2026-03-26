using System.Collections.Generic;
namespace Samqtt.SystemSensors
{
    public interface ISystemMultiSensor
    {
        string ConfigKey { get; }
        IEnumerable<string> ChildIdentifiers { get; }
    }
}
