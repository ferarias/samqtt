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

                actionInstance.Metadata = CreateMetadata(actionInstance.GetType(), actionName);

                yield return actionInstance;
            }
        }

        private SystemActionMetadata CreateMetadata(Type actionType, string actionName)
        {
            var am = new SystemActionMetadata
            {
                Key = actionName,
                Name = actionType.Name.Replace("Action", ""),
                UniqueId = topicProvider.GetUniqueId(actionName),
                DiscoveryTopic = topicProvider.GetActionResponseDiscoveryTopic(actionName),
                CommandTopic = topicProvider.GetActionCommandTopic(actionName),
                StateTopic = topicProvider.GetActionStateTopic(actionName),
                JsonAttributesTopic = topicProvider.GetActionJsonAttributesTopic(actionName)
            };
            return am;
        }
    }
}
