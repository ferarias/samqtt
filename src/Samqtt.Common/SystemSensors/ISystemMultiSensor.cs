using System.Collections.Generic;
namespace Samqtt.SystemSensors
{
    public interface ISystemMultiSensor
    {       
        IEnumerable<string> ChildIdentifiers { get; }
    }
}
