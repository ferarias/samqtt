using System.Reflection;
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
    ILogger<SystemActionFactory> logger) : ISystemActionFactory
    {
        private readonly SamqttOptions _options = options.Value;
        private static readonly Dictionary<string, (Type HandlerType, PropertyInfo? ReturnProperty)> _handlers = [];

        public IDictionary<string, ISystemAction> GetEnabledActions()
        {
            var allActions = serviceProvider.GetServices<ISystemAction>();
            var result = new Dictionary<string, ISystemAction>(StringComparer.OrdinalIgnoreCase);

            foreach (var (actionKey, actionOptions) in _options.Listeners)
            {
                if (!actionOptions.Enabled)
                {
                    logger.LogInformation("Action {Action} is disabled in configuration", actionKey);
                    continue;
                }

                var matching = allActions.FirstOrDefault(a => a.GetType().Name.Equals(actionKey + "Action", StringComparison.OrdinalIgnoreCase));

                if (matching == null)
                {
                    logger.LogWarning("No matching ISystemAction implementation found for key: {Action}", actionKey);
                    continue;
                }

                var topic = string.IsNullOrWhiteSpace(actionOptions.Topic)
                    ? SanitizeHelpers.Sanitize(actionKey)
                    : SanitizeHelpers.Sanitize(actionOptions.Topic);

                var uniqueId = $"{_options.DeviceUniqueId}_{SanitizeHelpers.Sanitize(actionKey)}";

                matching.Metadata = new SystemActionMetadata
                {
                    Key = actionKey,
                    Name = matching.GetType().Name.Replace("Action", ""),
                    UniqueId = uniqueId,
                    StateTopic = $"{_options.MqttBaseTopic}/{topic}",
                    CommandTopic = $"{_options.MqttBaseTopic}/{topic}"
                };

                result[actionKey] = matching;
            }

            return result;
        }
    }
}
