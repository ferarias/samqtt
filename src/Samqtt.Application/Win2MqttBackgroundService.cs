using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Samqtt.Options;
using Samqtt.SystemActions;
using Samqtt.SystemSensors;

namespace Samqtt.Application
{
    public class SamqttBackgroundService(
        IMqttConnectionManager connectionManager,
        ISystemActionFactory actionFactory,
        ISystemSensorFactory sensorFactory,
        IMessagePublisher publisher,
        IMqttSubscriber subscriber,
        IOptionsMonitor<SamqttOptions> options,
        ILogger<SamqttBackgroundService> logger) : BackgroundService
    {
        private readonly static SemaphoreSlim _semaphore = new(1, 1);
        private readonly IEnumerable<ISystemSensor> _activeSensors = sensorFactory.GetEnabledSensors();
        private readonly IEnumerable<ISystemAction> _activeActions = actionFactory.GetEnabledActions();


        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            // Connect to MQTT broker, Subscribe to incoming messages and Publish Home Assistant discovery messages/online status
            // Connect to MQTT broker
            await connectionManager.ConnectAsync(stoppingToken);


            // Publish Home Assistant sensor discovery messages
            foreach (var sensor in _activeSensors)
            {
                await publisher.PublishSensorDiscoveryMessage(sensor.Metadata, stoppingToken);
            }

            foreach (var action in _activeActions)
            {
                // Subscribe to incoming messages
                await subscriber.SubscribeAsync(action.Metadata.CommandTopic, action.HandleAsync, stoppingToken);
                // Publish Home Assistant button discovery message
                await publisher.PublishActionStateDiscoveryMessage(action.Metadata, stoppingToken);
            }

            // Publish online status
            await publisher.PublishOnlineStatus(stoppingToken);

            await base.StartAsync(stoppingToken);
            logger.LogInformation("Worker started");

        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    // Allow only one thread collecting system information
                    await _semaphore.WaitAsync(stoppingToken);
                    try
                    {

                        foreach (var sensor in _activeSensors)
                        {
                            try
                            {
                                var collectedValue = await sensor.CollectAsync();
                                await publisher.PublishSensorValue(sensor, collectedValue, stoppingToken);
                            }
                            catch (Exception ex)
                            {
                                logger.LogWarning(ex, "Failed to collect from sensor {Sensor}", sensor.Metadata.Name);
                            }
                        }
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
                    await Task.Delay(TimeSpan.FromSeconds(options.CurrentValue.TimerInterval), stoppingToken);
                }
            }
            catch (OperationCanceledException)
            {
                // When the stopping token is canceled, for example, a call made from services.msc,
                // we shouldn't exit with a non-zero exit code. In other words, this is expected...
                logger.LogInformation("Cancelling pending operations.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "{Message}", ex.Message);

                // Terminates this process and returns an exit code to the operating system.
                // This is required to avoid the 'BackgroundServiceExceptionBehavior', which
                // performs one of two scenarios:
                // 1. When set to "Ignore": will do nothing at all, errors cause zombie services.
                // 2. When set to "StopHost": will cleanly stop the host, and log errors.
                //
                // In order for the Windows Service Management system to leverage configured
                // recovery options, we need to terminate the process with a non-zero exit code.
                Environment.Exit(1);
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            // Disonnect from MQTT broker
            await publisher.PublishOfflineStatus(cancellationToken);

            // Publish offline status
            await connectionManager.DisconnectAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
            logger.LogInformation("Worker stopped.");
        }
    }
}
