using Mercury.Web.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Mercury.Web.Bootstrap
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseMercuryExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
    }
}