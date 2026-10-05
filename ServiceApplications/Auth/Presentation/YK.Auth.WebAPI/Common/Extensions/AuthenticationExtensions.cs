using Microsoft.AspNetCore.Authentication.JwtBearer;
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