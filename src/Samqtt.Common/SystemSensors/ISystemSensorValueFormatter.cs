using System.Diagnostics.CodeAnalysis;

namespace Samqtt.SystemSensors
{
    public interface ISystemSensorValueFormatter
    {
        [RequiresDynamicCode("Fallback branch may serialize unknown types via reflection.")]
        [RequiresUnreferencedCode("Fallback branch may serialize unknown types via reflection.")]
        public string Format<T>(T? value);

        [RequiresDynamicCode("Fallback branch may deserialize unknown struct types via reflection.")]
        [RequiresUnreferencedCode("Fallback branch may deserialize unknown struct types via reflection.")]
        public T? Format<T>(string? value) where T : struct;
    }
}
