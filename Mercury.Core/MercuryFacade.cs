using System.Threading.Tasks;
using Mercury.Abstraction.Interfaces;
using Mercury.Abstraction.Models;
using Mercury.Core.Settings;
using Mercury.Utils;
using Mercury.Validation;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Mercury.Core
{
    public class MercuryFacade : IMercuryFacade
    {
        private readonly IValidator validator;
        private readonly IPathFinder pathFinder;
        private readonly IResourceLoader resourceLoader;
        private readonly ITemplateProcessor templateProcessor;
        private readonly IEmailer emailer;
        private readonly MercurySettings settings;

        public MercuryFacade(
            IValidator validator,
            IPathFinder pathFinder,
            IResourceLoader resourceLoader,
            ITemplateProcessor templateProcessor,
            IEmailer emailer,
            IOptions<MercurySettings> options)
        {
            this.resourceLoader = resourceLoader.ThrowIfNull(nameof(resourceLoader));
            this.templateProcessor = templateProcessor.ThrowIfNull(nameof(templateProcessor));
            this.emailer = emailer.ThrowIfNull(nameof(emailer));
            this.pathFinder = pathFinder.ThrowIfNull(nameof(pathFinder));
            this.validator = validator.ThrowIfNull(nameof(validator));
            settings = options.ThrowIfNull(nameof(options)).Value;
            settings.ThrowIfNull(nameof(settings));
        }

        public async Task SendAsync(EmailRequest request)
        {
            validator.Validate(request);

            var body = await GetEmailBodyAsync(request);
            var metadata = await GetMetadataAsync(request);

            await SendAsync(request, metadata, body);
        }

        private async Task<string> GetEmailBodyAsync(EmailRequest request)
        {
            var templatePath = pathFinder.GetTemplatePath(request.TemplateKey);
            var template = await resourceLoader.LoadAsync(templatePath);
            var emailBody = templateProcessor.Process(template, request.Payload);

            return emailBody;
        }

        private async Task<EmailMetadata> GetMetadataAsync(EmailRequest request)
        {
            var metadataPath = pathFinder.GetMetadataPath(request.TemplateKey);
            var metadata = await resourceLoader.LoadAsync(metadataPath);
            var metadataObj = JsonConvert.DeserializeObject<EmailMetadata>(metadata);

            return metadataObj;
        }

        private async Task SendAsync(EmailRequest request, EmailMetadata metadata, string body)
        {
            var msg = Map(request, metadata, body);

            await emailer.SendAsync(msg);
        }

        private EmailMessage Map(EmailRequest request, EmailMetadata metadata, string body)
        {
            return new EmailMessage
            {
                From = metadata.From ?? settings.From,
                Tos = request.Tos,
                Ccs = request.Ccs,
                Bccs = request.Bccs,
                Subject = metadata.Subject,
                Body = body
            };
        }
    }
}