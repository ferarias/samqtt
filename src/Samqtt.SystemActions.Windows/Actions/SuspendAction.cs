namespace Samqtt.SystemActions.Windows.Actions
{
    public class SuspendAction : SystemAction<Unit>
    {
        public override string ConfigKey => "Suspend";

        public override Task<Unit> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            WindowsPowerManagement.SuspendSystem();
            return Task.FromResult(Unit.Default);
        }
    }

}
