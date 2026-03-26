using System.Text.Json.Serialization;
using Samqtt.SystemActions.Actions;

namespace Samqtt.SystemActions
{
    [JsonSerializable(typeof(CommandParameters))]
    internal partial class SamqttActionsJsonContext : JsonSerializerContext { }
}
