using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace YK.Auth.WebAPI.Common.Extensions
{
    public static class AuthenticationExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var issuer = GetRequired(configuration, "Jwt:Issuer");
            var audience = GetRequired(configuration, "Jwt:Audience");
            var signingAlgorithm = GetRequired(configuration, "Jwt:SigningAlgorithm");

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = issuer,
                ValidAudience = audience,

                RoleClaimType = "role"
            };

            switch (signingAlgorithm.ToUpperInvariant())
            {
                case "HS256":
                    ConfigureHs256Validation(tokenValidationParameters, configuration);
                    break;

                case "RS256":
                    ConfigureRs256Validation(tokenValidationParameters, configuration);
                    break;

                default:
                    throw new NotSupportedException($"Signing algorithm '{signingAlgorithm}' is not supported.");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                    .AddJwtBearer(options =>
                    {
                        options.MapInboundClaims = false;
                        options.TokenValidationParameters = tokenValidationParameters;

                        options.Events = new JwtBearerEvents
                        {
                            // 401: no token, invalid token, or expired token
                            OnChallenge = async context =>
                            {
                                context.HandleResponse(); // stop the default empty 401

                                var detail = context.AuthenticateFailure switch
                                {
                                    SecurityTokenExpiredException => "Access token has expired.",
                                    null => "Authentication is required to access this resource.",
                                    _ => "Access token is invalid."
                                };

                                await WriteProblemAsync(context.HttpContext, StatusCodes.Status401Unauthorized, "unauthorized", "Unauthorized", detail);
                            },

                            // 403: valid token, but the user lacks the required role
                            OnForbidden = context => WriteProblemAsync(context.HttpContext, StatusCodes.Status403Forbidden, "forbidden", "Forbidden", "You do not have permission to perform this action.")
                        };
                    });

            return services;
        }

        private static void ConfigureHs256Validation(TokenValidationParameters parameters, IConfiguration configuration)
        {
            var secretKey = GetRequired(configuration, "Jwt:Hs256:SecretKey");

            parameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        private static void ConfigureRs256Validation(TokenValidationParameters parameters, IConfiguration configuration)
        {
            var publicKey = GetRequired(configuration, "Jwt:Rs256:PublicKey");

            // No 'using': the key is used for every request, so it must live for the app's lifetime
            var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);

            parameters.IssuerSigningKey = new RsaSecurityKey(rsa);
        }

        // Writes 401/403 in the same ProblemDetails format as ExceptionHandlingMiddleware
        private static async Task WriteProblemAsync(HttpContext httpContext, int status, string type, string title, string detail)
        {
            httpContext.Response.StatusCode = status;

            var problemDetailsService = httpContext.RequestServices.GetRequiredService<IProblemDetailsService>();

            await problemDetailsService.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails = new ProblemDetails
                {
                    Type = type,
                    Title = title,
                    Status = status,
                    Detail = detail,
                    Instance = httpContext.Request.Path
                }
            });
        }

        // Fails at startup if the value is missing OR empty (e.g. "" in appsettings.json)
        private static string GetRequired(IConfiguration configuration, string key)
        {
            var value = configuration[key];

            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidOperationException($"{key} is not configured.");

            return value;
        }
    }
}