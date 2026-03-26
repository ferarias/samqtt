using System.Collections.Generic;

namespace Samqtt.Options
{
    public class SystemMultiSensorOptions
    {
        public bool Enabled { get; set; } = true;

        public Dictionary<string, SystemSensorOptions> Sensors { get; set; } = [];

    }
}
