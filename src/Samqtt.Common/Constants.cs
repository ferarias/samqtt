using System.IO;

namespace Samqtt
{
    public static class Constants
    {
        public const string AppId = "SAMQTT";

        public const string ServiceName = "SAMQTT Service";

        public static readonly string UserAppSettingsFileName = $"{AppId.ToLowerInvariant()}.appsettings.json";

        public const string HomeAssistantTopic = "homeassistant";
    }
}