using System.Threading;
using System.Threading.Tasks;

namespace Mercury.Messaging.Abstractions
{
    public interface IRequestConsumer<TRequest>
    {
        void Start();

        Task<RequestMessage<TRequest>> ConsumeAsync(CancellationToken token);

        void Acknowledge(RequestMessage<TRequest> message);

        void Reject(RequestMessage<TRequest> message);
    }
}