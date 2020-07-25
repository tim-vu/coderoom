namespace Infrastructure.RabbitMq
{
    public class RabbitMqOptions
    {
        public const string RabbitMq = "RabbitMq";
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string Hostname { get; set; }

        public int Port { get; set; } = 5672;

        public string VirtualHost { get; set; } = "/";
    }
}