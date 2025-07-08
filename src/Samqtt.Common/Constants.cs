using System.IO;

namespace Samqtt
{
    public static class Constants
    {
        /// <summary>
        /// An identifier of the application. Keep it in lowercase.
        /// </summary>
        public const string AppId = "samqtt";

        public const string ServiceName = "SAMQTT Service";

        public static readonly string UserAppSettingsFileName = $"{AppId.ToLowerInvariant()}.appsettings.json";

        public const string HomeAssistantTopic = "homeassistant";
    }
}