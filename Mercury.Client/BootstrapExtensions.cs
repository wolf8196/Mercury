using System;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Client
{
    public static class BootstrapExtensions
    {
        public static IServiceCollection AddMercuryClient(this IServiceCollection services, Action<MercuryClientSettings> configure)
        {
            var settings = new MercuryClientSettings();
            configure(settings);
            services.AddSingleton(settings);
            services.AddSingleton<IMercuryClientFactory, MercuryClientFactory>();
            services.AddHttpClient<IMercuryClient, MercuryClient>(config =>
            {
                config.BaseAddress = new Uri(settings.BaseAddress);
            });

            return services;
        }
    }
}