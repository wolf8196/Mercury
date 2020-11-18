using System.Threading;
using System.Threading.Tasks;
using Mercury.Core.Abstractions;
using Mercury.Messaging.Abstractions;
using Mercury.Models;
using Mercury.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Mercury.Service.Workers
{
    public class MessageWorker : BackgroundService
    {
        private readonly IRequestConsumer<MercuryRequest> consumer;
        private readonly IMercuryFacade mercuryFacade;
        private readonly ILogger logger;

        public MessageWorker(IRequestConsumer<MercuryRequest> consumer, IMercuryFacade mercuryFacade, ILogger<MessageWorker> logger)
        {
            this.consumer = consumer;
            this.mercuryFacade = mercuryFacade;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            consumer.Start();

            while (!token.IsCancellationRequested)
            {
                var message = await consumer.ConsumeAsync(token).ConfigureAwait(false);

                var requestLogger = logger
                    .WithScope("MessageId", message.Id)
                    .WithScope("TraceId", message.CorrelationId);

                using (requestLogger.BeginScope())
                {
                    requestLogger.LogDebug("Starting to process queued message.");

                    var result = await mercuryFacade.SendAsync(message.Request, token).ConfigureAwait(false);

                    if (result.IsFailed)
                    {
                        requestLogger.LogError("Failed to process queued message.");
                        requestLogger.LogDebug("Rejecting queued message.");

                        consumer.Reject(message);

                        requestLogger.LogDebug("Rejected queued message.");
                    }
                    else
                    {
                        requestLogger.LogDebug("Processed queued message.");
                        requestLogger.LogDebug("Acknowledging queued message.");

                        consumer.Acknowledge(message);

                        requestLogger.LogDebug("Acknowledged queued message.");
                    }
                }
            }
        }
    }
}