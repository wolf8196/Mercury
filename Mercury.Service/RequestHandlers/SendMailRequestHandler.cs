using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Mercury.Core.Abstractions;
using Mercury.Models;

namespace Mercury.Service.CommandHandlers
{
    public class SendMailRequest : IRequest<Result>
    {
        public SendMailRequest(MercuryRequest<ExpandoObject> request)
        {
            Request = request;
        }

        public MercuryRequest<ExpandoObject> Request { get; }
    }

    public class SendMailRequestHandler : IRequestHandler<SendMailRequest, Result>
    {
        private readonly IMercuryFacade mercuryFacade;

        public SendMailRequestHandler(IMercuryFacade mercuryFacade)
        {
            this.mercuryFacade = mercuryFacade;
        }

        public Task<Result> Handle(SendMailRequest request, CancellationToken cancellationToken)
        {
            return mercuryFacade.SendAsync(request.Request, cancellationToken);
        }
    }
}