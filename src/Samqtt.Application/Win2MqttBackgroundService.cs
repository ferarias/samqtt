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
        ISystemSensorValueFormatter sensorValueFormatter,
        IOptionsMonitor<SamqttOptions> options,
        ILogger<SamqttBackgroundService> logger) : BackgroundService
    {
        private readonly static SemaphoreSlim _semaphore = new(1, 1);
        private IEnumerable<ISystemSensor> _activeSensors = [];
        private IEnumerable<ISystemAction> _activeActions = [];


        public override async Task StartAsync(CancellationToken stoppingToken)
        {
            // Connect to MQTT broker first, then load sensors and actions
            await connectionManager.ConnectAsync(stoppingToken);

            _activeSensors = sensorFactory.GetEnabledSensors();
            _activeActions = actionFactory.GetEnabledActions();

            // Publish Home Assistant sensor discovery messages
            foreach (var sensor in _activeSensors)
            {
                await publisher.PublishSensorStateDiscoveryMessage(sensor.Metadata, stoppingToken);
            }

            foreach (var action in _activeActions)
            {
                // Subscribe to incoming messages
                await subscriber.SubscribeAsync(action.Metadata, action.HandleAsync, stoppingToken);
                // Publish Home Assistant discovery message
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
                using var timer = new PeriodicTimer(TimeSpan.FromSeconds(options.CurrentValue.TimerInterval));
                while (await timer.WaitForNextTickAsync(stoppingToken))
                {
                    // Allow only one thread collecting system information
                    await _semaphore.WaitAsync(stoppingToken);
                    try
                    {
                        foreach (var sensor in _activeSensors)
                        {
                            stoppingToken.ThrowIfCancellationRequested();
                            try
                            {
                                var collectedValue = await sensor.CollectAsync();
                                await publisher.PublishSensorValue(sensor.Metadata.StateTopic, sensorValueFormatter.FormatObject(collectedValue), stoppingToken);
                            }
                            catch (Exception ex) when (ex is not OperationCanceledException)
                            {
                                logger.LogWarning(ex, "Failed to collect from sensor {Sensor}", sensor.Metadata.Name);
                            }
                        }
                    }
                    finally
                    {
                        _semaphore.Release();
                    }
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
            // Stop ExecuteAsync first so sensor collection is not racing with MQTT cleanup.
            await base.StopAsync(cancellationToken);

            await publisher.PublishOfflineStatus(cancellationToken);
            await connectionManager.DisconnectAsync(cancellationToken);
            logger.LogInformation("Worker stopped.");
        }
    }
}
