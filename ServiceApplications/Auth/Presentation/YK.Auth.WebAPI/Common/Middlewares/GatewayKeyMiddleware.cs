using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using YK.Auth.WebAPI.Common.Options;

namespace YK.Auth.WebAPI.Common.Middlewares
{
    public sealed class GatewayKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GatewayKeyMiddleware> _logger;
        private readonly byte[] _expectedKey;

        public const string HeaderName = "X-Gateway-Key";

        public GatewayKeyMiddleware(RequestDelegate next, IOptions<GatewayOptions> options, ILogger<GatewayKeyMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _expectedKey = Encoding.UTF8.GetBytes(options.Value.SharedSecret);
        }

        public async Task InvokeAsync(HttpContext context, IProblemDetailsService problemDetailsService)
        {
            // Health endpoints bypass gateway key validation for health monitoring.
            // If enabled in production, ensure they expose no sensitive information.

            if (context.Request.Path.StartsWithSegments("/health") || context.Request.Path.StartsWithSegments("/alive"))
            {
                await _next(context);
                return;
            }

            if (!context.Request.Headers.TryGetValue(HeaderName, out var provided) || !CryptographicOperations.FixedTimeEquals(_expectedKey, Encoding.UTF8.GetBytes(provided.ToString())))
            {
                _logger.LogWarning("Gateway authentication failed. TraceId: {TraceId}", context.TraceIdentifier);

                context.Response.StatusCode = StatusCodes.Status403Forbidden;

                await problemDetailsService.WriteAsync(
                    new ProblemDetailsContext
                    {
                        HttpContext = context,
                        ProblemDetails = new ProblemDetails
                        {
                            Status = StatusCodes.Status403Forbidden,
                            Title = "Forbidden",
                            Detail = "Invalid or missing gateway key."
                        }
                    });

                return;
            }

            await _next(context);
        }
    }

}

