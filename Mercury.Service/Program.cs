using Mercury.Service.Settings;
using Mercury.Service.Workers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Enrichers.Span;

namespace Mercury.Service
{
    public static class Program
    {
        public static void Main()
        {
            var host = Host
                .CreateDefaultBuilder()
                .UseSerilog((hostBuilderContext, loggerConfig) =>
                {
                    loggerConfig.ReadFrom
                        .Configuration(hostBuilderContext.Configuration)
                        .Enrich.WithSpan();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureServices((hostBuilderContext, services) =>
                {
                    var serviceSettings = hostBuilderContext.Configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    if (serviceSettings.StartMessageWorker)
                    {
                        services.AddHostedService<MessageWorker>();
                    }
                })
                .Build();

            host.Run();
        }
    }
}