using Mercury.Service.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Mercury.Service.Bootstrap
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMercuryExceptionHandling(this IApplicationBuilder builder)
        {
            builder.UseMiddleware<ExceptionHandlingMiddleware>();
            return builder;
        }
    }
}