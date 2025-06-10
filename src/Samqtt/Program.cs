using Serilog;
using Samqtt.Application;
using Samqtt.Broker.MQTTNet;
using Samqtt.HomeAssistant;
using Samqtt.Options;
using Samqtt.SystemSensors;
using Samqtt.SystemActions;
using Samqtt;


#if WINDOWS
using Samqtt.SystemSensors.Windows;
using Samqtt.SystemActions.Windows;
#endif

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

#if WINDOWS
// Manage installation and uninstallation of the service (CliWrap)
if (await WindowsServiceInstaller.HandleServiceInstallationAsync(args))
    return;
#endif

try
{
    Log.Information("Start application.");

    var builder = Host.CreateApplicationBuilder(args);
#if !DEBUG
#if WINDOWS
    var appDataConfigPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), Constants.AppId, Constants.UserAppSettingsFile);
    Log.Information("Applying custom settings from {ConfigPath}", appDataConfigPath);
    builder.Configuration.AddJsonFile(appDataConfigPath, optional: true);
#else
    var appDataConfigPath = Path.Combine("/etc/", Constants.AppId.ToLowerInvariant(), Constants.UserAppSettingsFile);
    Log.Information("Applying custom settings from {ConfigPath}", appDataConfigPath);
    builder.Configuration.AddJsonFile(appDataConfigPath, optional: true);
#endif
#endif

    builder.Services
        .AddHostedService<SamqttBackgroundService>();

    builder.Services
        .AddSerilog((services, loggerConfiguration) => loggerConfiguration
            .ReadFrom.Configuration(builder.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console());

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
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
