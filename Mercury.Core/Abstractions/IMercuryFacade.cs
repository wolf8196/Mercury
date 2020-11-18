using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Mercury.Models;

namespace Mercury.Core.Abstractions
{
    public interface IMercuryFacade
    {
        Task<Result> SendAsync(MercuryRequest request, CancellationToken token);
    }
}