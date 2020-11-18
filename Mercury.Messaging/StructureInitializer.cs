using System.Collections.Generic;
using Mercury.Messaging.Abstractions;
using Mercury.Utils;
using RabbitMQ.Client;

namespace Mercury.Messaging
{
    internal class StructureInitializer : IStructureInitializer
    {
        private readonly object lockObject;
        private readonly RabbitSettings settings;

        private bool isInitialized;

        public StructureInitializer(RabbitSettings settings)
        {
            lockObject = new object();
            this.settings = settings.ThrowIfNull(nameof(settings));
        }

        public void Initialize(IModel channel)
        {
            if (isInitialized)
            {
                return;
            }

            lock (lockObject)
            {
                if (isInitialized)
                {
                    return;
                }

                channel.ExchangeDeclare(
                    settings.ExchangeName,
                    ExchangeType.Direct,
                    true,
                    false);

                channel.QueueDeclare(
                    queue: settings.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    new Dictionary<string, object>
                    {
                        { "x-dead-letter-exchange", settings.ExchangeName },
                        { "x-dead-letter-routing-key", settings.RejectRoutingKey }
                    });

                channel.QueueDeclare(
                    queue: settings.ErrorQueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false);

                channel.QueueBind(
                    settings.QueueName,
                    settings.ExchangeName,
                    settings.PublishRoutingKey);

                channel.QueueBind(
                    settings.ErrorQueueName,
                    settings.ExchangeName,
                    settings.RejectRoutingKey);

                isInitialized = true;
            }
        }
    }
}