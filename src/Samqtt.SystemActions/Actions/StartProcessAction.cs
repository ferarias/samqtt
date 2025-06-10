using System.Diagnostics;
using System.Text.Json;

namespace Samqtt.SystemActions.Actions
{
    public class StartProcessAction : SystemAction<Unit>
    {
        public override async Task<Unit> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            var commandParameters = JsonSerializer.Deserialize<CommandParameters>(payload);
            if (commandParameters != null)
            {
                ProcessWindowStyle processWindowStyle = commandParameters.WindowStyle switch
                {
                    0 => ProcessWindowStyle.Normal,
                    1 => ProcessWindowStyle.Hidden,
                    2 => ProcessWindowStyle.Minimized,
                    3 => ProcessWindowStyle.Maximized,
                    _ => ProcessWindowStyle.Normal,
                };
                var startInfo = new ProcessStartInfo(commandParameters.CommandString, commandParameters.ExecParameters)
                {
                    WindowStyle = processWindowStyle

                };
                var runningProcess = Process.Start(startInfo) ?? throw new InvalidOperationException($"Failed to start process: {commandParameters.CommandString}");
                await runningProcess.WaitForExitAsync(cancellationToken);
            }
            return Unit.Default;
        }
    }

    internal class CommandParameters
    {
        public required string CommandString { get; set; }
        public int WindowStyle { get; set; }
        public string ExecParameters { get; set; } = string.Empty;
        public string? MonitorId { get; set; }
    }
}


//{
//    "CommandString": "notepad.exe",
//    "WindowStyle": 0,
//    "ExecParameters": "%WINDOWS%\\System32\drivers\etc\hosts"
//}