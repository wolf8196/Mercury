namespace Mercury.Messaging
{
    public class RabbitSettings
    {
        public string Host { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ExchangeName { get; set; }

        public string QueueName => $"{ExchangeName}-worker";

        public string ErrorQueueName => $"{ExchangeName}-error";

        public string PublishRoutingKey => "publish";

        public string RejectRoutingKey => "reject";
    }
}