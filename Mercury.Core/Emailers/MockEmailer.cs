using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Mercury.Core.Abstractions;
using Mercury.Core.Models;

namespace Mercury.Core.Emailers
{
    public class MockEmailer : IEmailer
    {
        public Task<Result> SendAsync(EmailMessage message, CancellationToken token)
        {
            return Task.FromResult(Result.Ok());
        }
    }
}