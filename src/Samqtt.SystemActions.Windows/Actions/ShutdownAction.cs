namespace Samqtt.SystemActions.Windows.Actions
{
    public class ShutdownAction : SystemAction<Unit>
    {
        public override string ConfigKey => "Shutdown";

        private static readonly int DefaultShutdownDelay = 10;

        public override Task<Unit> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            if (int.TryParse(payload, out int shutdownDelay))
                WindowsPowerManagement.Shutdown(shutdownDelay);
            else
                WindowsPowerManagement.Shutdown(DefaultShutdownDelay);

            return Task.FromResult(Unit.Default);
        }
    }

}
