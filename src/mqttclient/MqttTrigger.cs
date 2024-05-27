﻿using System.Xml.Serialization;

namespace Win2Mqtt.Client
{
    [XmlRoot(ElementName = "mqtttriggers")]
    public class MqttTrigger
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CmdText { get; set; }
        public string CmdParameters { get; set; }
        public bool Predefined { get; set; }
    }
}
