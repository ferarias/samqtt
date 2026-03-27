using System.Diagnostics.CodeAnalysis;

namespace Samqtt.SystemSensors
{
    public interface ISystemSensorValueFormatter
    {
        /// <summary>
        /// AOT-safe formatting for boxed sensor values. Handles all primitive types
        /// (bool, double, float, int, long, decimal, DateTime, DateTimeOffset) via
        /// pattern matching. Falls back to <see cref="object.ToString"/> for unknown types.
        /// </summary>
        public string FormatObject(object? value);

        [RequiresDynamicCode("Fallback branch may serialize unknown types via reflection.")]
        [RequiresUnreferencedCode("Fallback branch may serialize unknown types via reflection.")]
        public string Format<T>(T? value);

        [RequiresDynamicCode("Fallback branch may deserialize unknown struct types via reflection.")]
        [RequiresUnreferencedCode("Fallback branch may deserialize unknown struct types via reflection.")]
        public T? Format<T>(string? value) where T : struct;
    }
}
