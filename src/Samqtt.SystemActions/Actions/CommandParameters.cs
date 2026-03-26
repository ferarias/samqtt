namespace Samqtt.SystemActions.Actions
{
    internal class CommandParameters
    {
        public required string CommandString { get; set; }
        public int WindowStyle { get; set; }
        public string ExecParameters { get; set; } = string.Empty;
        public string? MonitorId { get; set; }
    }
}
