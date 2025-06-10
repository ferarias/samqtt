using System.Diagnostics;

namespace Samqtt.SystemActions.Actions
{
    public class KillProcessAction : SystemAction<bool>
    {
        public override Task<bool> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            try
            {
                foreach (var process in Process.GetProcessesByName(payload))
                {
                    process.Kill();
                }
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error killing process: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}
