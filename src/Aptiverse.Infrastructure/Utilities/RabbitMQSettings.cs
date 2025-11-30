namespace Aptiverse.Infrastructure.Utilities
{
    public class RabbitMQSettings
    {
        public string? Host { get; set; }
        public int Port { get; set; } = 5672;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? VirtualHost { get; set; } = "/";
        public string? QueueName { get; set; } = "email_queue";
    }
}
