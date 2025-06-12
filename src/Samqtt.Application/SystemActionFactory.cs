using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samqtt.Common;
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
                    logger.LogInformation("Action {Action} is disabled in configuration", actionName);
                    continue;
                }

                var actionInstance = allActions
                    .FirstOrDefault(a => a.GetType().Name.Equals(actionName + "Action", StringComparison.OrdinalIgnoreCase));

                if (actionInstance == null)
                {
                    logger.LogWarning("No actionInstance ISystemAction implementation found for key: {Action}", actionName);
                    continue;
                }

                var topic = string.IsNullOrWhiteSpace(actionOptions.Topic)
                    ? SanitizeHelpers.Sanitize(actionName)
                    : SanitizeHelpers.Sanitize(actionOptions.Topic);

                var uniqueId = topicProvider.GetUniqueId(actionName);

                actionInstance.Metadata = new SystemActionMetadata
                {
                    Key = actionName,
                    Name = actionInstance.GetType().Name.Replace("Action", ""),
                    UniqueId = uniqueId,
                    StateTopic = topicProvider.GetStateTopic(topic),
                    CommandTopic = topicProvider.GetStateTopic(topic)
                };

                yield return actionInstance;
            }
        }
    }
}
