namespace Samqtt.SystemActions.Windows.Actions
{
    public class HibernateAction : SystemAction<Unit>
    {
        public override Task<Unit> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            WindowsPowerManagement.HibernateSystem();
            return Task.FromResult(Unit.Default);
        }
    }

}
