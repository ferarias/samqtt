namespace Samqtt
{
    public static class MessageExtensions
    {
        public static string BooleanToMqttOneOrZero(this bool? value) => value.HasValue && value.Value ? "1" : "0";
        public static string BooleanToMqttOneOrZero(this bool value) => value ? "1" : "0";

        public static string BooleanToMqttOnOff(this bool? value) => value.HasValue && value.Value ? "on" : "off";
        public static string BooleanToMqttOnOff(this bool value) => value ? "on" : "off";

        public static string BooleanToMqttYesNo(this bool? value) => value.HasValue && value.Value ? "yes" : "no";
        public static string BooleanToMqttYesNo(this bool value) => value ? "yes" : "no";

        public static int? MqttAsNullableInt(this string message) => int.TryParse(message, out var result) ? result : null;
        public static int MqttAsInt(this string message, int defaultValue = default) => int.TryParse(message, out var result) ? result : defaultValue;

        public static bool? MqttAsBoolean(this string value) => value switch
        {
            "1" => true,
            "0" => false,
            _ => null,
        };

        public static bool? MqttAsOnOff(this string value) => value?.Trim().ToLowerInvariant() switch
        {
            "on" => true,
            "off" => false,
            _ => null,
        };

        public static bool? MqttAsYesNo(this string value) => value?.Trim().ToLowerInvariant() switch
        {
            "yes" => true,
            "no" => false,
            _ => null,
        };

        public static bool? MqttAsAnyBoolean(this string value) => 
            value.MqttAsBoolean()
            ?? value.MqttAsOnOff()
            ?? value.MqttAsYesNo();
    }
}