using System.Threading;
using System.Threading.Tasks;
using Mercury.Models;

namespace Mercury.Client
{
    public interface IMercuryClient
    {
        Task<MercuryResult> SendAsync<TPayload>(MercuryRequest<TPayload> request, CancellationToken token = default)
            where TPayload : class;

        Task<MercuryResult> QueueAsync<TPayload>(MercuryRequest<TPayload> request, CancellationToken token = default)
            where TPayload : class;
    }
}