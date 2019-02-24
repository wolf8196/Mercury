using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mercury.ServiceSettings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace Mercury.Middleware
{
    public class KeyAuthenticationMiddleware : IMiddleware
    {
        private readonly IOptions<ServiceSettingsModel> settings;
        private const string AuthSchema = "Bearer";
        private const int RegexTokenValueGroupNumber = 2;

        public KeyAuthenticationMiddleware(IOptions<ServiceSettingsModel> settings)
        {
            this.settings = settings;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var hasAuthHeader = context.Request.Headers.TryGetValue("Authorization", out StringValues values);
            var regex = new Regex($"^({AuthSchema})\\s([a-zA-Z0-9]+)$", RegexOptions.IgnoreCase);

            if (hasAuthHeader)
            {
                var matchResult = regex.Match(values.First());

                if (matchResult.Success
                    && matchResult.Groups[RegexTokenValueGroupNumber].Value == settings.Value.AuthenticationKey)
                {
                    await next(context);
                    return;
                }
            }

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }
    }
}