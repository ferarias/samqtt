using System.Diagnostics;

namespace Samqtt.SystemActions.Actions
{
    public class ShutdownAction : SystemAction<Unit>
    {
        public override string ConfigKey => "Shutdown";

        public override async Task<Unit> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            var startInfo = new ProcessStartInfo("systemctl", "poweroff")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start systemctl poweroff");
            await process.WaitForExitAsync(cancellationToken);
            return Unit.Default;
        }
    }
}
