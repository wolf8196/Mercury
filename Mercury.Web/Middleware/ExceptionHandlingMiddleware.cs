using System;
using System.Threading.Tasks;
using Mercury.ResourceLoaders.Exceptions;
using Mercury.TemplateProcessors.Exceptions;
using Mercury.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mercury.Web.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlingMiddleware> logger;

        public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
        {
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ResourceNotFoundException ex)
            {
                LogError(ex);
                SetResponse(context, StatusCodes.Status404NotFound);
            }
            catch (TemplateProcessingException ex)
            {
                LogError(ex);
                SetResponse(context, StatusCodes.Status400BadRequest);
            }
            catch (ModelValidationException ex)
            {
                LogError(ex);
                SetResponse(context, StatusCodes.Status400BadRequest);
            }
            catch (Exception ex)
            {
                LogError(ex);
                SetResponse(context, StatusCodes.Status500InternalServerError);
            }
        }

        private void SetResponse(HttpContext context, int statusCode)
        {
            context.Response.Clear();
            context.Response.StatusCode = statusCode;
        }

        private void LogError(Exception ex)
        {
            logger.LogError(ex, "Mercury threw an exception");
        }
    }
}