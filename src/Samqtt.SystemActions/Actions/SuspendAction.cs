using System.Diagnostics;

namespace Samqtt.SystemActions.Actions
{
    public class SuspendAction : SystemAction<Unit>
    {
        public override string ConfigKey => "Suspend";

        public override async Task<Unit> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            var startInfo = new ProcessStartInfo("systemctl", "suspend")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start systemctl suspend");
            await process.WaitForExitAsync(cancellationToken);
            return Unit.Default;
        }
    }
}
