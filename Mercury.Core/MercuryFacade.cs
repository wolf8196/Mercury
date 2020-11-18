using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using FluentValidation;
using Mercury.Core.Abstractions;
using Mercury.Core.Models;
using Mercury.Models;
using Mercury.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Mercury.Core
{
    public class MercuryFacade : IMercuryFacade
    {
        private const string EmailRequestNullFailureMessage = "Email request cannot be null.";
        private const string EmailRequestInvalidFailureMessage = "Email request is invalid.";

        private readonly IValidator<MercuryRequest> validator;
        private readonly IPathFinder pathFinder;
        private readonly IResourceLoader resourceLoader;
        private readonly ITemplateProcessor templateProcessor;
        private readonly IEmailer emailer;
        private readonly ILogger logger;
        private readonly MercurySettings settings;

        public MercuryFacade(
            IValidator<MercuryRequest> validator,
            IPathFinder pathFinder,
            IResourceLoader resourceLoader,
            ITemplateProcessor templateProcessor,
            IEmailer emailer,
            ILogger<MercuryFacade> logger,
            MercurySettings settings)
        {
            this.resourceLoader = resourceLoader.ThrowIfNull(nameof(resourceLoader));
            this.templateProcessor = templateProcessor.ThrowIfNull(nameof(templateProcessor));
            this.emailer = emailer.ThrowIfNull(nameof(emailer));
            this.pathFinder = pathFinder.ThrowIfNull(nameof(pathFinder));
            this.validator = validator.ThrowIfNull(nameof(validator));
            this.logger = logger.ThrowIfNull(nameof(logger));
            this.settings = settings.ThrowIfNull(nameof(settings));
        }

        public async Task<Result> SendAsync(MercuryRequest request, CancellationToken token)
        {
            logger.WithScope("@Request", request).LogDebug("Starting to process email request.");

            var validationResult = Validate(request);
            if (validationResult.IsFailed)
            {
                return validationResult;
            }

            token.ThrowIfCancellationRequested();

            logger.LogDebug("Generating email body.");

            var bodyResult = await GetEmailBodyAsync(request, token).ConfigureAwait(false);
            if (bodyResult.IsFailed)
            {
                return bodyResult;
            }

            logger.WithScope("Body", bodyResult.Value).LogDebug("Generated email body.");

            token.ThrowIfCancellationRequested();

            logger.LogDebug("Extracting metadata.");

            var metadataResult = await GetMetadataAsync(request, token).ConfigureAwait(false);
            if (metadataResult.IsFailed)
            {
                return metadataResult;
            }

            logger.WithScope("@Metadata", metadataResult.Value).LogDebug("Extracted metadata.");

            token.ThrowIfCancellationRequested();

            logger.LogDebug("Mapping to message.");

            var message = Map(request, metadataResult.Value, bodyResult.Value);

            logger.WithScope("@Message", message).LogDebug("Mapped to message.");

            token.ThrowIfCancellationRequested();

            logger.LogDebug("Sending email.");

            return await emailer.SendAsync(message, token).ConfigureAwait(false);
        }

        private Result Validate(MercuryRequest request)
        {
            if (request == null)
            {
                logger.LogError(EmailRequestNullFailureMessage);
                return Result.Fail(EmailRequestNullFailureMessage);
            }

            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(x => x.ErrorMessage);
                logger.WithErrorScope(errors).LogError(EmailRequestInvalidFailureMessage);
                return Result.Fail(new Error(EmailRequestInvalidFailureMessage).CausedBy(errors));
            }

            return Result.Ok();
        }

        private async Task<Result<string>> GetEmailBodyAsync(MercuryRequest request, CancellationToken token)
        {
            var templatePath = pathFinder.GetTemplatePath(request.TemplateKey);

            var templateResult = await resourceLoader.LoadAsync(templatePath, token).ConfigureAwait(false);
            if (templateResult.IsFailed)
            {
                return templateResult;
            }

            token.ThrowIfCancellationRequested();

            var templateProcessingResult = templateProcessor.Process(templateResult.Value, request.Payload);
            if (templateProcessingResult.IsFailed)
            {
                return templateProcessingResult;
            }

            return Result.Ok(templateProcessingResult.Value);
        }

        private async Task<Result<EmailMetadata>> GetMetadataAsync(MercuryRequest request, CancellationToken token)
        {
            var metadataPath = pathFinder.GetMetadataPath(request.TemplateKey);

            var resourceResult = await resourceLoader.LoadAsync(metadataPath, token).ConfigureAwait(false);
            if (resourceResult.IsFailed)
            {
                return resourceResult.ToResult<EmailMetadata>();
            }

            token.ThrowIfCancellationRequested();

            var metadataObj = JsonConvert.DeserializeObject<EmailMetadata>(resourceResult.Value);

            return Result.Ok(metadataObj);
        }

        private EmailMessage Map(MercuryRequest request, EmailMetadata metadata, string body)
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