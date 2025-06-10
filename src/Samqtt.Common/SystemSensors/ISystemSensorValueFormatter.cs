namespace Samqtt.SystemSensors
{
    public interface ISystemSensorValueFormatter
    {
        public string Format<T>(T? value);
        public T? Format<T>(string? value) where T : struct;
    }
}
