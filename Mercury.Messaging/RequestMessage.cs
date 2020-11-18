using System;

namespace Mercury.Messaging
{
    public class RequestMessage<TRequest>
    {
        public RequestMessage(
            TRequest request,
            DateTimeOffset timestamp = default,
            string correlationId = default,
            string id = default)
        {
            Id = id ?? Guid.NewGuid().ToString();
            CorrelationId = correlationId;
            Timestamp = timestamp;
            Request = request;
        }

        public string Id { get; set; }

        public string CorrelationId { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public TRequest Request { get; }
    }
}