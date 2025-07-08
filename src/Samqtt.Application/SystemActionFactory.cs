using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Samqtt.Options;
using Samqtt.SystemActions;

namespace Samqtt.Application
{
    public class SystemActionFactory(
        IOptions<SamqttOptions> options,
        IServiceProvider serviceProvider,
        ITopicProvider topicProvider,
        ILogger<SystemActionFactory> logger) : ISystemActionFactory
    {
        private readonly SamqttOptions _options = options.Value;

        public IEnumerable<ISystemAction> GetEnabledActions()
        {
            var allActions = serviceProvider.GetServices<ISystemAction>();

            foreach (var (actionName, actionOptions) in _options.Actions)
            {
                if (!actionOptions.Enabled)
                {
                    logger.LogDebug("Action {Action} is disabled in configuration.", actionName);
                    continue;
                }

                var actionInstance = allActions.FirstOrDefault(a => a.GetType().Name.Equals(actionName + "Action", StringComparison.OrdinalIgnoreCase));
                if (actionInstance == null)
                {
                    logger.LogWarning("No actionInstance ISystemAction implementation found for key: {Action}", actionName);
                    continue;
                }

                if (actionInstance is SystemAction<Unit> systemAction)
                {
                    actionInstance.Metadata = CreateMetadata(systemAction.GetType(), actionName, false);
                }
                else
                {
                    actionInstance.Metadata = CreateMetadata(actionInstance.GetType(), actionName, true);
                }

                yield return actionInstance;
            }
        }

        private SystemActionMetadata CreateMetadata(Type actionType, string actionName, bool returnsState) =>
            new()
            {
                Key = actionName,
                Name = actionType.Name.Replace("Action", ""),
                UniqueId = topicProvider.GetUniqueId(actionName),
                DiscoveryTopic = topicProvider.GetActionResponseDiscoveryTopic(actionName),
                CommandTopic = topicProvider.GetActionCommandTopic(actionName),
                StateTopic = returnsState ? topicProvider.GetActionStateTopic(actionName) : null,
                JsonAttributesTopic = returnsState ? topicProvider.GetActionJsonAttributesTopic(actionName) : null
            };
    }
}
