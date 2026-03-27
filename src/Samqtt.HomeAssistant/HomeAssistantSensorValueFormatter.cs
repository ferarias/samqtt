using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.Json;
using Samqtt.SystemSensors;

namespace Samqtt.HomeAssistant
{
    public class HomeAssistantSensorValueFormatter : ISystemSensorValueFormatter
    {
        private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.General)
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };

        public string FormatObject(object? value) => value switch
        {
            null => string.Empty,
            bool b => b ? "1" : "0",
            DateTime dt => dt.ToUniversalTime().ToString("O"),
            DateTimeOffset dto => dto.ToUniversalTime().ToString("O"),
            double d => d.ToString("0.##", CultureInfo.InvariantCulture),
            float f => f.ToString("0.##", CultureInfo.InvariantCulture),
            int i => i.ToString(CultureInfo.InvariantCulture),
            long l => l.ToString(CultureInfo.InvariantCulture),
            decimal dec => dec.ToString(CultureInfo.InvariantCulture),
            string s => s,
            _ => value.ToString() ?? string.Empty,
        };

        [RequiresDynamicCode("Fallback branch serializes unknown types via reflection. All sensor value types should be handled by explicit branches.")]
        [RequiresUnreferencedCode("Fallback branch serializes unknown types via reflection. All sensor value types should be handled by explicit branches.")]
        public string Format<T>(T? value)
        {
            if (value is null) return string.Empty;

            return value switch
            {
                bool b => b ? "1" : "0",
                DateTime dt => dt.ToUniversalTime().ToString("O"),
                DateTimeOffset dto => dto.ToUniversalTime().ToString("O"),
                double d => d.ToString("0.##", CultureInfo.InvariantCulture),
                float f => f.ToString("0.##", CultureInfo.InvariantCulture),
                int i => i.ToString(CultureInfo.InvariantCulture),
                long l => l.ToString(CultureInfo.InvariantCulture),
                decimal dec => dec.ToString(CultureInfo.InvariantCulture),
                IEnumerable<object> enumerable => SerializeFallback(enumerable),
                _ => SerializeFallback(value)
            };
        }

        [RequiresDynamicCode("Fallback branch deserializes unknown struct types via reflection. All known struct types are handled by explicit branches.")]
        [RequiresUnreferencedCode("Fallback branch deserializes unknown struct types via reflection. All known struct types are handled by explicit branches.")]
        public T? Format<T>(string? value) where T : struct
        {
            if (string.IsNullOrEmpty(value))
                return default;

            var targetType = typeof(T);

            try
            {
                if (targetType == typeof(bool))
                    return (T)(object)(value == "1" || value.Equals("true", StringComparison.OrdinalIgnoreCase));
                if (targetType == typeof(DateTime))
                    return (T)(object)DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                if (targetType == typeof(DateTimeOffset))
                    return (T)(object)DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
                if (targetType == typeof(double))
                    return (T)(object)double.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(float))
                    return (T)(object)float.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(int))
                    return (T)(object)int.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(long))
                    return (T)(object)long.Parse(value, CultureInfo.InvariantCulture);
                if (targetType == typeof(decimal))
                    return (T)(object)decimal.Parse(value, CultureInfo.InvariantCulture);

                // Fall back to JSON deserialization for unknown struct types.
                // This path is dead for all currently known callers (all primitives handled above).
                return DeserializeFallback<T>(value);
            }
            catch
            {
                return default;
            }
        }

        [RequiresDynamicCode("Fallback JSON serialization uses reflection. All known sensor value types are handled by explicit branches above.")]
        [RequiresUnreferencedCode("Fallback JSON serialization uses reflection. All known sensor value types are handled by explicit branches above.")]
        private static string SerializeFallback<T>(T value) => JsonSerializer.Serialize(value, JsonOptions);

        [RequiresDynamicCode("Fallback JSON deserialization uses reflection. All known struct types are handled by explicit branches above.")]
        [RequiresUnreferencedCode("Fallback JSON deserialization uses reflection. All known struct types are handled by explicit branches above.")]
        private static T? DeserializeFallback<T>(string value) where T : struct =>
            JsonSerializer.Deserialize<T>(value, JsonOptions);
    }

}
