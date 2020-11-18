using System.Diagnostics;
using Mercury.Service.Settings;
using Mercury.Service.Workers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Mercury.Service
{
    public static class Program
    {
        public static void Main()
        {
            Activity.DefaultIdFormat = ActivityIdFormat.W3C;

            var host = Host
                .CreateDefaultBuilder()
                .UseSerilog((hostBuilderContext, loggerConfig) =>
                {
                    loggerConfig.ReadFrom.Configuration(hostBuilderContext.Configuration);
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