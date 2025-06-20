using Samqtt.Application;
using Samqtt.Broker.MQTTNet;
using Samqtt.HomeAssistant;
using Samqtt.Options;
using Samqtt.SystemActions;
using Samqtt.SystemSensors;
using Serilog;
using Samqtt;

#if WINDOWS
using Samqtt.SystemActions.Windows;
using Samqtt.SystemSensors.Windows;
using Serilog.Events;
#endif

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
#if WINDOWS
    .WriteTo.EventLog(Constants.ServiceName, restrictedToMinimumLevel: LogEventLevel.Information)
#endif
    .CreateBootstrapLogger();

#if WINDOWS
if (await WindowsServiceInstaller.HandleServiceInstallationAsync(args))
    return;
#endif

try
{
    Log.Information($"{Constants.ServiceName} is starting.");
    var builder = Host.CreateApplicationBuilder(args);

    if (File.Exists(EnvironmentConstants.UserAppSettingsFile))
    {
        Log.Information("Applying custom settings from {ConfigPath}", EnvironmentConstants.UserAppSettingsFile);
        builder.Configuration.AddJsonFile(EnvironmentConstants.UserAppSettingsFile, optional: true);
    }
    builder.Services
        .AddHostedService<SamqttBackgroundService>();

    builder.Services
        .AddSerilog((services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console()
#if WINDOWS
            .WriteTo.EventLog(Constants.ServiceName, restrictedToMinimumLevel: LogEventLevel.Error)
#endif
            );

    builder.Services
        .AddSamqttOptions()
        .AddSamqttApplication()
        .AddMqtt2NetBroker()
        .AddHomeAssistant()
        .AddSystemActions()
        .AddSystemSensors();
#if WINDOWS
    Log.Information("Adding Windows-specific sensors and actions");
    builder.Services
        .AddWindowsSpecificSystemSensors()
        .AddWindowsSpecificSystemActions()
        .AddWindowsService(options => options.ServiceName = $"{Constants.AppId} Service");
#endif

    await builder.Build().RunAsync();

    Log.Information($"{Constants.ServiceName} ended normally.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
