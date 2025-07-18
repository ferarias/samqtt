﻿namespace Samqtt.SystemActions
{
    public class SystemActionMetadata : MetadataBase
    {
        /// <summary>
        /// This is the topic where the action can be triggered.
        /// </summary>
        public required string CommandTopic { get; set; }

        /// <summary>
        /// When a value is returned, this topic is used to publish the state.
        /// </summary>
        public string? StateTopic { get; set; }

        /// <summary>
        /// When a value is returned, this topic is used to publish full data returned.
        /// </summary>
        public string? JsonAttributesTopic { get; set; }

    }
}
