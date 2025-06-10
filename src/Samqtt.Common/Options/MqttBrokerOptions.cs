using System.ComponentModel.DataAnnotations;

namespace Samqtt.Options
{
    public class MqttBrokerOptions
    {
        [Required]
        public required string Server { get; set; }
        public int? Port { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}
