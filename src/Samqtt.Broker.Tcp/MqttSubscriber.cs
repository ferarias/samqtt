using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samqtt.Options;
using Samqtt.SystemActions;
using Samqtt.SystemSensors;
using System.Text.Json;

namespace Samqtt.Broker.Tcp;

internal class MqttSubscriber : IMqttSubscriber
{
    private readonly Dictionary<string, Func<string, CancellationToken, Task<object?>>> _actions = [];
    private readonly Dictionary<string, (string StateTopic, string JsonAttributesTopic)> _returnTopics = [];

    private readonly MqttTcpClient _client;
    private readonly SamqttOptions _options;
    private readonly ISystemActionFactory actionFactory;
    private readonly ILogger<MqttSubscriber> logger;

    public MqttSubscriber(
        MqttTcpClient client,
        IOptions<SamqttOptions> options,
        ISystemActionFactory actionFactory,
        IMessagePublisher publisher,
        ISystemSensorValueFormatter sensorValueFormatter,
        IHostApplicationLifetime appLifetime,
        ILogger<MqttSubscriber> logger)
    {
        this.actionFactory = actionFactory;
        this.logger = logger;
        _client = client;
        _options = options.Value;

        var stoppingToken = appLifetime.ApplicationStopping;

        _client.MessageReceived = async (commandTopic, commandPayload) =>
        {
            logger.LogDebug("Message received in MQTT commandTopic `{Topic}`.", commandTopic);
            try
            {
                var result = await _actions[commandTopic].Invoke(commandPayload, stoppingToken);
                var stateTopic = _returnTopics[commandTopic].StateTopic;
                var jsonAttrTopic = _returnTopics[commandTopic].JsonAttributesTopic;

                if (result is IEnumerable<object> enumerable)
                {
                    var items = enumerable.Select(item => sensorValueFormatter.FormatObject(item)).ToList();
                    var count = items.Count;
                    await publisher.PublishActionStateValue(stateTopic, $"Returned {count} items", stoppingToken);
                    var resultPayload = new ActionResultPayload(count, items);
                    await publisher.PublishActionStateValue(
                        jsonAttrTopic,
                        JsonSerializer.Serialize(resultPayload, SamqttBrokerJsonContext.Default.ActionResultPayload),
                        stoppingToken);
                }
                else
                {
                    var v = sensorValueFormatter.FormatObject(result);
                    await publisher.PublishActionStateValue(stateTopic, v, stoppingToken);
                    await publisher.PublishActionStateValue(jsonAttrTopic, v, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Message received MQTT commandTopic `{Topic}` caused an exception.", commandTopic);
            }
        };
    }

    public async Task<bool> SubscribeAsync(SystemActionMetadata metadata, Func<string, CancellationToken, Task<object?>> handler, CancellationToken cancellationToken = default)
    {
        var commandTopic = metadata.CommandTopic;
        string stateTopic = metadata.StateTopic!;
        string jsonAttrTopic = metadata.JsonAttributesTopic!;
        do
        {
            try
            {
                if (_client?.IsConnected != true) return false;

                await _client.SubscribeAsync(commandTopic, cancellationToken);
                logger.LogDebug("Subscribe to MQTT commandTopic `{Topic}`. Success.", commandTopic);

                _actions.Add(commandTopic, handler);
                _returnTopics.Add(commandTopic, (stateTopic, jsonAttrTopic));
                return true;
            }
            catch (Exception ex)
            {
                logger.LogCritical(ex, "Subscribe to MQTT commandTopic `{Topic}`. Exception!", commandTopic);
            }

            logger.LogWarning("Subscribe to MQTT commandTopic `{Topic}`. Could not subscribe; retrying in 10 seconds...", commandTopic);
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

        } while (!cancellationToken.IsCancellationRequested);

        return false;
    }
}
