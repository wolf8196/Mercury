using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Mercury.Messaging.Abstractions;
using Mercury.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mercury.Messaging
{
    internal sealed class RequestConsumer<TRequest> : IRequestConsumer<TRequest>, IDisposable
    {
        private readonly IConnectionFactory connectionFactory;
        private readonly IStructureInitializer initializer;
        private readonly ILogger logger;
        private readonly RabbitSettings settings;
        private readonly BufferBlock<RequestMessage<TRequest>> messageBuffer;
        private readonly ConcurrentDictionary<RequestMessage<TRequest>, ulong> deliveryTags;

        private IConnection connection;
        private IModel channel;
        private EventingBasicConsumer consumer;

        public RequestConsumer(
            IConnectionFactory connectionFactory,
            IStructureInitializer initializer,
            ILogger<RequestConsumer<TRequest>> logger,
            RabbitSettings settings)
        {
            this.connectionFactory = connectionFactory;
            this.initializer = initializer;
            this.logger = logger.WithScope("QueueHost", settings.Host).WithScope("QueueName", settings.QueueName);
            this.settings = settings;

            messageBuffer = new BufferBlock<RequestMessage<TRequest>>();
            deliveryTags = new ConcurrentDictionary<RequestMessage<TRequest>, ulong>();
        }

        public void Start()
        {
            logger.LogInformation("Starting queue consumer.");

            connection ??= connectionFactory.CreateConnection();
            channel ??= connection.CreateModel();

            initializer.Initialize(channel);

            consumer = new EventingBasicConsumer(channel);
            consumer.Received += OnConsumerReceived;

            channel.BasicQos(0, 1, false);

            channel.BasicConsume(settings.QueueName, false, consumer);

            logger.LogInformation("Started queue consumer.");
        }

        public async Task<RequestMessage<TRequest>> ConsumeAsync(CancellationToken token)
        {
            return await messageBuffer.ReceiveAsync(token).ConfigureAwait(false);
        }

        public void Acknowledge(RequestMessage<TRequest> message)
        {
            if (deliveryTags.TryRemove(message, out ulong deliveryTag))
            {
                channel.BasicAck(deliveryTag, false);
            }
        }

        public void Reject(RequestMessage<TRequest> message)
        {
            if (deliveryTags.TryRemove(message, out ulong deliveryTag))
            {
                channel.BasicReject(deliveryTag, false);
            }
        }

        public void Dispose()
        {
            channel?.Dispose();
            connection?.Dispose();
        }

        private void OnConsumerReceived(object sender, BasicDeliverEventArgs args)
        {
            var requestStr = Encoding.UTF8.GetString(args.Body.Span);
            var request = JsonConvert.DeserializeObject<TRequest>(requestStr);
            var message = new RequestMessage<TRequest>(
                request,
                DateTimeOffset.FromUnixTimeSeconds(args.BasicProperties.Timestamp.UnixTime),
                args.BasicProperties.CorrelationId,
                args.BasicProperties.MessageId);

            deliveryTags.TryAdd(message, args.DeliveryTag);
            messageBuffer.Post(message);
        }
    }
}