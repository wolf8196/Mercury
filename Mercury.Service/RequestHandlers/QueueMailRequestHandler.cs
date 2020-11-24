using System;
using System.Diagnostics;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Mercury.Messaging;
using Mercury.Messaging.Abstractions;
using Mercury.Models;
using Mercury.Utils;
using Microsoft.Extensions.Logging;

namespace Mercury.Service.CommandHandlers
{
    public class QueueMailRequest : IRequest<Result>
    {
        public QueueMailRequest(MercuryRequest<ExpandoObject> request)
        {
            Request = request;
        }

        public MercuryRequest<ExpandoObject> Request { get; }
    }

    public class QueueMailRequestHandler : IRequestHandler<QueueMailRequest, Result>
    {
        private const string PublishFailureMessage = "Failed to queue email request.";

        private readonly IRequestPublisher<MercuryRequest<ExpandoObject>> publisher;
        private readonly ILogger logger;

        public QueueMailRequestHandler(IRequestPublisher<MercuryRequest<ExpandoObject>> publisher, ILogger<QueueMailRequestHandler> logger)
        {
            this.publisher = publisher;
            this.logger = logger;
        }

        public Task<Result> Handle(QueueMailRequest request, CancellationToken cancellationToken)
        {
            var requestMessage = new RequestMessage<MercuryRequest<ExpandoObject>>(
                request.Request,
                DateTimeOffset.UtcNow,
                Activity.Current.TraceId.ToString());

            var requestLogger = logger
                .WithScope("@Request", request)
                .WithScope("MessageId", requestMessage.Id);

            requestLogger.LogDebug("Queuing email request.");

            try
            {
                publisher.Publish(requestMessage);

                requestLogger.LogDebug("Queued email request.");
            }
            catch (Exception ex)
            {
                requestLogger.LogError(ex, PublishFailureMessage);
                return Task.FromResult(
                    Result.Fail(PublishFailureMessage));
            }

            return Task.FromResult(Result.Ok());
        }
    }
}