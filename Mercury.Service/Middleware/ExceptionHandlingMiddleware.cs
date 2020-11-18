using System;
using System.Net.Mime;
using System.Threading.Tasks;
using Mercury.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Mercury.Service.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private static readonly JsonSerializerSettings SerializationSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            }
        };

        private readonly ILogger logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                logger.LogDebug("Request was cancelled");

                context.Response.StatusCode = StatusCodes.Status204NoContent;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception occured");

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = MediaTypeNames.Application.Json;

                await context.Response
                    .WriteAsync(JsonConvert.SerializeObject(MercuryResult.InternalServerErrorResult(), SerializationSettings))
                    .ConfigureAwait(false);
            }
        }
    }
}