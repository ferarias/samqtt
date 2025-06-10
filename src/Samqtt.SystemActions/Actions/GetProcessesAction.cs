using System.Diagnostics;

namespace Samqtt.SystemActions.Actions
{
    public class GetProcessesAction : SystemAction<ProcessInfo[]>
    {
        public override Task<ProcessInfo[]> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            return Task.FromResult(
                Process
                .GetProcesses()
                .Select(p => new ProcessInfo(p.ProcessName, p.Id))
                .ToArray());
        }
    }

    public record ProcessInfo(string Name, int Id);
}
