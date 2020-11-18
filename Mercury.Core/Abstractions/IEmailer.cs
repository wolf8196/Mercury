using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Mercury.Core.Models;

namespace Mercury.Core.Abstractions
{
    public interface IEmailer
    {
        Task<Result> SendAsync(EmailMessage message, CancellationToken token);
    }
}