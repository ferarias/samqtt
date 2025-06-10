#if WINDOWS
using CliWrap;

using Serilog;
using Samqtt;

internal class WindowsServiceInstaller
{
    internal static async Task<bool> HandleServiceInstallationAsync(string[] args)
    {
        var argsList = args.Select(a => a.ToLowerInvariant()).ToList();
        if (argsList.Contains("/install"))
        {
            Log.Information("Install service.");
            await Cli.Wrap("sc")
                .WithArguments(["create", Constants.ServiceName, $"binPath=\"{Environment.ProcessPath}\"", "start=auto"])
                .ExecuteAsync();
            return true;
        }
        else if (argsList.Contains("/uninstall"))
        {
            Log.Information("Uninstall service.");
            await Cli.Wrap("sc")
                .WithArguments(["stop", Constants.ServiceName])
                .ExecuteAsync();

            await Cli.Wrap("sc")
                .WithArguments(["delete", Constants.ServiceName])
                .ExecuteAsync();
            return true;
        }
        return false;
    }
}
#endif

