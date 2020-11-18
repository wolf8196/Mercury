using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using Mercury.Core.Abstractions;
using Mercury.Core.Models;
using Mercury.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Errors.Model;
using SendGrid.Helpers.Mail;
using SendGridEmailAddress = SendGrid.Helpers.Mail.EmailAddress;

namespace Mercury.Core.Emailers
{
    public class SendGridEmailer : IEmailer
    {
        private const string SendGridFailureMessage = "Failed to send an email. SendGrid returned unexpected status code.";

        private readonly ILogger logger;
        private readonly SendGridSettings settings;

        public SendGridEmailer(ILogger<SendGridEmailer> logger, SendGridSettings settings)
        {
            this.logger = logger;
            this.settings = settings.ThrowIfNull(nameof(settings));
            settings.ApiKey.ThrowIfNullOrEmpty(nameof(settings.ApiKey));
        }

        public async Task<Result> SendAsync(EmailMessage message, CancellationToken token)
        {
            var client = new SendGridClient(new SendGridClientOptions
            {
                ApiKey = settings.ApiKey,
                HttpErrorAsException = true,
            });

            try
            {
                var response = await client.SendEmailAsync(Map(message), token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var error = JsonConvert.DeserializeObject<SendGridErrorResponse>(ex.Message);

                logger.WithErrorScope(error).LogError(SendGridFailureMessage);

                return Result.Fail(new Error(SendGridFailureMessage)
                    .CausedBy(new Error(error.SendGridErrorMessage)
                    .WithMetadata("StatusCode", error.ErrorHttpStatusCode)
                    .WithMetadata("HelpLink", error.HelpLink)));
            }

            return Result.Ok();
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