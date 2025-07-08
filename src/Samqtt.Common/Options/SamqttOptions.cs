using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Samqtt.Options
{
    public class SamqttOptions
    {
        public static readonly string SectionName = Constants.AppId.ToLowerInvariant();

        [Required]
        public required MqttBrokerOptions Broker { get; set; }

        [Required()]
        [RegularExpression(@"[^/\\#]+$")]
        public string DeviceIdentifier { get; set; } = Environment.MachineName;

        public int TimerInterval { get; set; } = 5;

        [Required]
        public required Dictionary<string, SystemSensorOptions> Sensors { get; set; } = [];
        [Required]
        public Dictionary<string, SystemMultiSensorOptions> MultiSensors { get; set; } = [];

        [Required]
        public Dictionary<string, SystemActionOptions> Actions { get; set; } = [];
    }
}
