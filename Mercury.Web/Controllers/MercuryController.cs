using System.Threading.Tasks;
using Mercury.Abstraction.Models;
using Mercury.Core;
using Microsoft.AspNetCore.Mvc;

namespace Mercury.Web.Controllers
{
    [Route("api")]
    public class MercuryController : ControllerBase
    {
        private readonly IMercuryFacade mercuryFacade;

        public MercuryController(IMercuryFacade mercuryFacade)
        {
            this.mercuryFacade = mercuryFacade;
        }

        [HttpGet("healthcheck")]
        public IActionResult HealthCheck()
        {
            return Ok();
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendMailAsync([FromBody]EmailRequest model)
        {
            await mercuryFacade.SendAsync(model);

            return Ok();
        }
    }
}