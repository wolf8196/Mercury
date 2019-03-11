using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Mercury.Abstraction.Interfaces;
using Mercury.Abstraction.Models;
using Mercury.Emailers.Exceptions;
using Mercury.Emailers.Settings;
using Mercury.Utils;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGridEmailAddress = SendGrid.Helpers.Mail.EmailAddress;

namespace Mercury.Services
{
    public class SendGridEmailer : IEmailer
    {
        private readonly SendGridSettings settings;

        public SendGridEmailer(IOptions<SendGridSettings> options)
        {
            settings = options.ThrowIfNull(nameof(options)).Value;
            settings.ThrowIfNull(nameof(settings));
            settings.ApiKey.ThrowIfNull(nameof(settings.ApiKey));
        }

        public async Task SendAsync(EmailMessage message)
        {
            var client = new SendGridClient(settings.ApiKey);

            var response = await client.SendEmailAsync(Map(message));

            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                throw new EmailingException(Emailers.Enums.EmailerTypes.SendGrid);
            }
        }

        private SendGridMessage Map(EmailMessage message)
        {
            var msg = new SendGridMessage();

            msg.SetFrom(new SendGridEmailAddress(message.From.Address, message.From.DisplayName));
            msg.AddTos(message.Tos.Select(x => new SendGridEmailAddress(x)).ToList());
            if (message.Ccs != null && message.Ccs.Any())
            {
                msg.AddCcs(message.Ccs.Select(x => new SendGridEmailAddress(x)).ToList());
            }

            if (message.Bccs != null && message.Ccs.Any())
            {
                msg.AddBccs(message.Bccs.Select(x => new SendGridEmailAddress(x)).ToList());
            }

            msg.SetSubject(message.Subject);
            msg.HtmlContent = message.Body;

            return msg;
        }
    }
}