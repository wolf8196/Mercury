using System;
using System.Threading.Tasks;
using Mercury.ResourceLoaders.Exceptions;
using Mercury.TemplateProcessors.Exceptions;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace Mercury.Web.Middleware
{
    public class ExceptionHandlingMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (ResourceNotFoundException ex)
            {
                await WriteResponse(
                    context,
                    ex,
                    StatusCodes.Status404NotFound,
                    new
                    {
                        ResourceLoaderType = ex.ResourceLoaderType.ToString(),
                        ex.Path
                    });
            }
            catch (TemplateProcessingException ex)
            {
                await WriteResponse(
                    context,
                    ex,
                    StatusCodes.Status400BadRequest,
                    new
                    {
                        TemplateProcessorType = ex.TemplateProcessorType.ToString(),
                        ex.Errors
                    });
            }
            catch (Exception ex)
            {
                await WriteResponse(context, ex, StatusCodes.Status500InternalServerError);
            }
        }

        private Task WriteResponse(
            HttpContext context,
            Exception ex,
            int statusCode,
            object additionalData = null)
        {
            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            return context.Response.WriteAsync(
                JsonConvert.SerializeObject(
                    new
                    {
                        Type = ex.GetType().Name,
                        ex.Message,
                        Data = additionalData
                    },
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    }));
        }
    }
}