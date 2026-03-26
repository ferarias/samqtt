using System.ComponentModel.DataAnnotations;

namespace Samqtt.Options
{
    public class MqttBrokerOptions
    {
        [Required]
        public string Server { get; set; } = string.Empty;
        public int? Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
