using System.Text.Json.Serialization;
using Samqtt.SystemActions.Actions;

namespace Samqtt.SystemActions
{
    [JsonSerializable(typeof(CommandParameters))]
    [JsonSerializable(typeof(NotificationParameters))]
    internal partial class SamqttActionsJsonContext : JsonSerializerContext { }
}
