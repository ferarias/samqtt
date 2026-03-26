using System.Diagnostics;
using System.Text.Json;

namespace Samqtt.SystemActions.Actions
{
    public class SendNotificationAction : SystemAction<Unit>
    {
        public override string ConfigKey => "SendNotification";

        public override async Task<Unit> HandleCoreAsync(string payload, CancellationToken cancellationToken)
        {
            var parameters = JsonSerializer.Deserialize(payload, SamqttActionsJsonContext.Default.NotificationParameters);
            if (parameters != null)
            {
                var args = string.IsNullOrWhiteSpace(parameters.Icon)
                    ? $"\"{parameters.Summary}\" \"{parameters.Body}\""
                    : $"--icon=\"{parameters.Icon}\" \"{parameters.Summary}\" \"{parameters.Body}\"";

                var startInfo = new ProcessStartInfo("notify-send", args)
                {
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };
                var process = Process.Start(startInfo) ?? throw new InvalidOperationException("Failed to start notify-send");
                await process.WaitForExitAsync(cancellationToken);
            }
            return Unit.Default;
        }
    }

    public record NotificationParameters(string Summary, string Body = "", string Icon = "");
}
