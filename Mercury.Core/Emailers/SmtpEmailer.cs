using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MailKit.Net.Smtp;
using Mercury.Core.Abstractions;
using Mercury.Core.Models;
using Mercury.Utils;
using MimeKit;

namespace Mercury.Core.Emailers
{
    public class SmtpEmailer : IEmailer
    {
        private readonly SmtpSettings settings;

        public SmtpEmailer(SmtpSettings settings)
        {
            this.settings = settings.ThrowIfNull(nameof(settings));
        }

        public async Task<Result> SendAsync(EmailMessage message, CancellationToken token)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(message.From.DisplayName, message.From.Address));
            msg.To.AddRange(message.Tos.Select(x => MailboxAddress.Parse(x)));

            if (message.Ccs != null && message.Ccs.Any())
            {
                msg.Cc.AddRange(message.Ccs.Select(x => MailboxAddress.Parse(x)));
            }

            if (message.Bccs != null && message.Ccs.Any())
            {
                msg.Bcc.AddRange(message.Ccs.Select(x => MailboxAddress.Parse(x)));
            }

            msg.Subject = message.Subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = message.Body
            };

            msg.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                var isSecure = !string.IsNullOrEmpty(settings.Username) && !string.IsNullOrEmpty(settings.Password);

                await client.ConnectAsync(settings.Host, settings.Port, isSecure, token).ConfigureAwait(false);

                if (isSecure)
                {
                    await client.AuthenticateAsync(settings.Username, settings.Password, token).ConfigureAwait(false);
                }

                await client.SendAsync(msg, token).ConfigureAwait(false);

                await client.DisconnectAsync(true, token).ConfigureAwait(false);
            }

            return Result.Ok();
        }
    }
}