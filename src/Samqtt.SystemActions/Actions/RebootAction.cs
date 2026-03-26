using System.Diagnostics;

namespace Samqtt.SystemActions.Actions
{
    public class RebootAction : SystemAction<Unit>
    {
        public override string ConfigKey => "Reboot";

        public override async Task<Unit> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            var startInfo = new ProcessStartInfo("systemctl", "reboot")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start systemctl reboot");
            await process.WaitForExitAsync(cancellationToken);
            return Unit.Default;
        }
    }
}
