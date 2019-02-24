using Mercury.Abstraction.Interfaces;
using Mercury.Core;
using Mercury.Core.Settings;
using Mercury.Emailers.Enums;
using Mercury.Emailers.Settings;
using Mercury.PathFinders;
using Mercury.ResourceLoaders;
using Mercury.ResourceLoaders.Enums;
using Mercury.ResourceLoaders.Settings;
using Mercury.Services;
using Mercury.TemplateProcessors;
using Mercury.TemplateProcessors.Enums;
using Mercury.Validation;
using Mercury.Web.Middleware;
using Mercury.Web.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mercury.Web.Bootstrap
{
    public static class DependencyExtensions
    {
        public static IServiceCollection AddMercuryDependencies(
            this IServiceCollection services,
            IHostingEnvironment environment,
            IConfiguration configuration)
        {
            services.AddMercurySettings(configuration);
            services.AddStaticDependencies();
            services.AddConfigBasedDependencies();

            return services;
        }

        private static IServiceCollection AddMercurySettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<MercurySettings>(
                configuration.GetSection("MercurySettings"));
            services.Configure<SendGridSettings>(
                configuration.GetSection("EmailerSettings:SendGridSettings"));
            services.Configure<LocalResourceSettings>(
                configuration.GetSection("ResourceLoaderSettings:LocalResourceSettings"));
            services.Configure<AzureBlobResourceSettings>(
                configuration.GetSection("ResourceLoaderSettings:AzureBlobResourceSettings"));
            services.Configure<ServicesConfig>(
                configuration.GetSection("StartupSettings:ServicesConfig"));

            return services;
        }

        private static IServiceCollection AddStaticDependencies(this IServiceCollection services)
        {
            services.AddSingleton<IValidator, MercuryValidator>();
            services.AddSingleton<IPathFinder, ConstantPathFinder>();
            services.AddSingleton<IMercuryFacade, MercuryFacade>();
            services.AddSingleton<ExceptionHandlingMiddleware>();

            return services;
        }

        private static IServiceCollection AddConfigBasedDependencies(this IServiceCollection services)
        {
            var config = services.BuildServiceProvider().GetService<IOptions<ServicesConfig>>().Value;

            switch (config.ResourceLoader)
            {
                case ResourceLoaderTypes.Local:
                    services.AddSingleton<IResourceLoader, LocalResourceLoader>();
                    break;

                case ResourceLoaderTypes.AzureBlob:
                    services.AddSingleton<IResourceLoader, AzureBlobResourceLoader>();
                    break;
            }

            switch (config.TemplateProcessor)
            {
                case TemplateProcessorTypes.Liquid:
                    services.AddSingleton<ITemplateProcessor, LiquidTemplateProcessor>();
                    break;

                case TemplateProcessorTypes.Handlebars:
                    services.AddSingleton<ITemplateProcessor, HandlebarsTemplateProcessor>();
                    break;
            }

            switch (config.Emailer)
            {
                case EmailerTypes.Dev:
                    services.AddSingleton<IEmailer, DevEmailer>();
                    break;

                case EmailerTypes.SendGrid:
                    services.AddSingleton<IEmailer, SendGridEmailer>();
                    break;
            }

            return services;
        }
    }
}