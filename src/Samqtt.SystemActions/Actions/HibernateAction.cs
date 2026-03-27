using System.Diagnostics;

namespace Samqtt.SystemActions.Actions
{
    public class HibernateAction : SystemAction<Unit>
    {
        public override string ConfigKey => "Hibernate";

        public override async Task<Unit> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            var startInfo = new ProcessStartInfo("systemctl", "hibernate")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
            var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start systemctl hibernate");
            await process.WaitForExitAsync(cancellationToken);
            return Unit.Default;
        }
    }
}
