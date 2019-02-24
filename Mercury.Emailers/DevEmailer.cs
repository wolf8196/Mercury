using System.Threading.Tasks;
using Mercury.Abstraction.Interfaces;
using Mercury.Abstraction.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Mercury.Services
{
    public class DevEmailer : IEmailer
    {
        private readonly ILogger<DevEmailer> logger;

        public DevEmailer(ILogger<DevEmailer> logger)
        {
            this.logger = logger;
        }

        public Task SendAsync(EmailMessage message)
        {
            logger.LogInformation("Sending email message: {Message}", JsonConvert.SerializeObject(message, Formatting.Indented));
            return Task.CompletedTask;
        }
    }
}