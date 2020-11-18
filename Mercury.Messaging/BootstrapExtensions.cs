using System;
using Mercury.Messaging.Abstractions;
using Mercury.Models;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace Mercury.Messaging
{
    public static class BootstrapExtensions
    {
        public static IServiceCollection AddMercuryMessaging(this IServiceCollection services, Action<RabbitSettings> configure)
        {
            services.AddSingleton(serviceProvider =>
            {
                var settings = new RabbitSettings();
                configure(settings);
                return settings;
            });

            services.AddSingleton<IConnectionFactory>(serviceProvider =>
            {
                var settings = serviceProvider.GetRequiredService<RabbitSettings>();
                return new ConnectionFactory
                {
                    HostName = settings.Host,
                    UserName = settings.Username,
                    Password = settings.Password,
                };
            });
            services.AddSingleton<IStructureInitializer, StructureInitializer>();
            services.AddSingleton<IRequestPublisher<MercuryRequest>, RequestPublisher<MercuryRequest>>();
            services.AddSingleton<IRequestConsumer<MercuryRequest>, RequestConsumer<MercuryRequest>>();

            return services;
        }
    }
}