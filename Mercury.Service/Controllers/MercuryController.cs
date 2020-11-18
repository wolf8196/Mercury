using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Mercury.Models;
using Mercury.Service.CommandHandlers;
using Microsoft.AspNetCore.Mvc;

namespace Mercury.Service.Controllers
{
    [ApiController]
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}")]
    public class MercuryController : ControllerBase
    {
        private readonly IMediator mediator;

        public MercuryController(IMediator mediator)
        {
            this.mediator = mediator;
        }

        [HttpGet("healthcheck")]
        public IActionResult HealthCheck()
        {
            return Ok(MercuryResult.OkResult());
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMailAsync([FromBody] MercuryRequest request, CancellationToken token)
        {
            var sendResult = await mediator.Send(new SendMailRequest(request), token).ConfigureAwait(false);

            if (sendResult.IsFailed)
            {
                return BadRequest(MercuryResult.BadRequestResult(sendResult));
            }

            return Ok(MercuryResult.OkResult());
        }

        [HttpPost("queue")]
        public async Task<IActionResult> QueueMailAsync([FromBody] MercuryRequest request, CancellationToken token)
        {
            var sendResult = await mediator.Send(new QueueMailRequest(request), token).ConfigureAwait(false);

            if (sendResult.IsFailed)
            {
                return BadRequest(MercuryResult.BadRequestResult(sendResult));
            }

            return Accepted(MercuryResult.AcceptedResult());
        }
    }
}