using System.Reflection;
using MediatR;
using Mercury.Messaging;
using Mercury.Service.Bootstrap;
using Mercury.Service.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mercury.Service
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddControllers()
                .AddNewtonsoftJson();

            services
                .AddApiVersioning()
                .AddMediatR(typeof(Startup).GetTypeInfo().Assembly)
                .AddMercuryCore(configuration)
                .AddMercuryMessaging(config =>
                {
                    configuration.GetSection(nameof(ServiceSettings)).GetSection(nameof(RabbitSettings)).Bind(config);
                });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMercuryExceptionHandling();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}