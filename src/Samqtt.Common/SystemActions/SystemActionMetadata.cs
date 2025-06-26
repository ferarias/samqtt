namespace Samqtt.SystemActions
{
    public class SystemActionMetadata : MetadataBase
    {
        /// <summary>
        /// This is the topic where the action can be triggered.
        /// </summary>
        public required string CommandTopic { get; set; }
    }
}
