namespace Samqtt
{
    public abstract class MetadataBase
    {
        /// <summary>
        /// Gets the unique key associated with this Action or Sensor.
        /// The key comes from the configuration and is used to identify the action/sensor.
        /// </summary>
        public required string Key { get; set; } = default!;

        /// <summary>
        /// Name of the action/sensor, e.g., "Reboot", "FreeMemory".
        /// It is the name of the type without the "Action" or "(Multi)Sensor" suffix.
        /// </summary>
        public required string Name { get; set; } = default!;

        /// <summary>
        /// The Unique ID is used to uniquely identify across Home Assistant and MQTT.
        /// </summary>
        public required string UniqueId { get; set; }

        /// <summary>
        /// When a value is returned, this topic is used to publish the state.
        /// </summary>
        public required string StateTopic { get; set; }

        /// <summary>
        /// This is the topic where the action/sensor is discovered in Home Assistant.
        /// </summary>
        public required string DiscoveryTopic { get; set; }

    }
}
