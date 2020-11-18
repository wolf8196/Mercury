using System;
using System.Buffers;
using System.Net.Mime;
using System.Text;
using Mercury.Messaging.Abstractions;
using Newtonsoft.Json;
using RabbitMQ.Client;

namespace Mercury.Messaging
{
    internal sealed class RequestPublisher<TRequest> : IRequestPublisher<TRequest>, IDisposable
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly IStructureInitializer initializer;
        private readonly RabbitSettings settings;
        private IConnection connection;
        private IModel channel;

        public RequestPublisher(
            IConnectionFactory connectionFactory,
            IStructureInitializer initializer,
            RabbitSettings settings)
        {
            this.connectionFactory = connectionFactory;
            this.initializer = initializer;
            this.settings = settings;
        }

        public void Publish(RequestMessage<TRequest> message)
        {
            connection ??= connectionFactory.CreateConnection();
            channel ??= connection.CreateModel();

            initializer.Initialize(channel);

            var props = channel.CreateBasicProperties();
            props.ContentType = MediaTypeNames.Application.Json;
            props.Persistent = true;
            props.Timestamp = new AmqpTimestamp(message.Timestamp.ToUnixTimeSeconds());
            props.MessageId = message.Id;
            props.CorrelationId = message.CorrelationId;

            var requestStr = JsonConvert.SerializeObject(message.Request);
            var byteCount = Encoding.UTF8.GetByteCount(requestStr);

            using var memoryOwner = MemoryPool<byte>.Shared.Rent(byteCount);
            var body = Encoding.UTF8.GetBytes(requestStr, memoryOwner.Memory.Span);

            channel.BasicPublish(settings.ExchangeName, settings.PublishRoutingKey, props, memoryOwner.Memory.Slice(0, byteCount));
        }

        public void Dispose()
        {
            channel?.Dispose();
            connection?.Dispose();
        }
    }
}