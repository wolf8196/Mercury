using System;
using System.Dynamic;
using FluentValidation;
using Mercury.Core;
using Mercury.Core.Abstractions;
using Mercury.Core.Emailers;
using Mercury.Core.PathFinders;
using Mercury.Core.ResourceLoaders;
using Mercury.Core.TemplateProcessors;
using Mercury.Core.Validation;
using Mercury.Models;
using Mercury.Service.Middleware;
using Mercury.Service.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Service.Bootstrap
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddMercuryCore(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services
                .AddMercurySettings(configuration)
                .AddStaticDependencies()
                .AddConfigBasedDependencies(configuration)
                .AddFluentValidationSettings();

            return services;
        }

        private static IServiceCollection AddMercurySettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration.GetSection(nameof(MercurySettings)).Get<MercurySettings>());
            services.AddSingleton(configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>());

            return services;
        }

        private static IServiceCollection AddStaticDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IValidator<MercuryRequest<ExpandoObject>>, MercuryRequestValidator>();
            services.AddSingleton<IPathFinder, ConstantPathFinder>();
            services.AddSingleton<IMercuryFacade, MercuryFacade>();
            services.AddTransient<ExceptionHandlingMiddleware>();

            return services;
        }

        private static IServiceCollection AddConfigBasedDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            var config = services.BuildServiceProvider().GetService<ServiceSettings>();

            switch (config.ResourceLoader)
            {
                case ResourceLoaderTypes.Local:
                    services.AddSingleton(configuration.GetSection("ResourceLoaderSettings:LocalResourceSettings").Get<LocalResourceSettings>());
                    services.AddSingleton<IResourceLoader, LocalResourceLoader>();
                    break;

                case ResourceLoaderTypes.AzureBlob:
                    services.AddSingleton(configuration.GetSection("ResourceLoaderSettings:AzureBlobResourceSettings").Get<AzureBlobResourceSettings>());
                    services.AddSingleton<IResourceLoader, AzureBlobResourceLoader>();
                    break;

                default:
                    throw new ArgumentException($"Resource loader was not found. Resource loader: {config.ResourceLoader}");
            }

            switch (config.TemplateProcessor)
            {
                case TemplateProcessorTypes.Liquid:
                    services.AddSingleton<ITemplateProcessor, LiquidTemplateProcessor>();
                    break;

                case TemplateProcessorTypes.Handlebars:
                    services.AddSingleton<ITemplateProcessor, HandlebarsTemplateProcessor>();
                    break;

                default:
                    throw new ArgumentException($"Template processor was not found. Template processor: {config.TemplateProcessor}");
            }

            switch (config.Emailer)
            {
                case EmailerTypes.Mock:
                    services.AddSingleton<IEmailer, MockEmailer>();
                    break;

                case EmailerTypes.SendGrid:
                    services.AddSingleton(configuration.GetSection("EmailerSettings:SendGridSettings").Get<SendGridSettings>());
                    services.AddSingleton<IEmailer, SendGridEmailer>();
                    break;

                case EmailerTypes.Smtp:
                    services.AddSingleton(configuration.GetSection("EmailerSettings:SmtpSettings").Get<SmtpSettings>());
                    services.AddSingleton<IEmailer, SmtpEmailer>();
                    break;

                default:
                    throw new ArgumentException($"Emailer was not found. Emailer: {config.Emailer}");
            }

            return services;
        }

        private static IServiceCollection AddFluentValidationSettings(this IServiceCollection services)
        {
            ValidatorOptions.Global.LanguageManager.Enabled = false;

            return services;
        }
    }
}