using System.Diagnostics;

namespace Samqtt.SystemActions.Actions
{
    public class GetProcessAction : SystemAction<bool>
    {
        public override Task<bool> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            try
            {
                var isRunning = Process.GetProcessesByName(payload)?.Length != 0 == true;
                return Task.FromResult(isRunning);
            }
            catch (Exception ex)
            {

                Console.WriteLine($"Error checking if process is running: {ex.Message}");
                return Task.FromResult(false);
            }
        }
    }
}
