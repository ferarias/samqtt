namespace Samqtt.SystemActions.Windows.Actions
{
    public class RebootAction : SystemAction<Unit>
    {
        private static readonly int DefaultRebootDelay = 10;

        public override Task<Unit> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            if (int.TryParse(payload, out int rebootDelay))
                WindowsPowerManagement.Reboot(rebootDelay);
            else
                WindowsPowerManagement.Reboot(DefaultRebootDelay);

            return Task.FromResult(Unit.Default);
        }
    }

}
