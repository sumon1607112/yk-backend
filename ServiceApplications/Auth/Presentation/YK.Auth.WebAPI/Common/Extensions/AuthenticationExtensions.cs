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
            var issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
            var audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");
            var signingAlgorithm = configuration["Jwt:SigningAlgorithm"] ?? throw new InvalidOperationException("Jwt:SigningAlgorithm is not configured.");


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
                        options.TokenValidationParameters = tokenValidationParameters;
                    });

            return services;
        }

        private static void ConfigureHs256Validation(TokenValidationParameters parameters, IConfiguration configuration)
        {
            var secretKey = configuration["Jwt:Hs256:SecretKey"] ?? throw new InvalidOperationException("Jwt:Hs256:SecretKey is not configured.");

            parameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        }

        private static void ConfigureRs256Validation(TokenValidationParameters parameters, IConfiguration configuration)
        {
            var publicKey = configuration["Jwt:Rs256:PublicKey"] ?? throw new InvalidOperationException("Jwt:Rs256:PublicKey is not configured.");
            using var rsa = RSA.Create();

            rsa.ImportFromPem(publicKey);

            parameters.IssuerSigningKey = new RsaSecurityKey(rsa);
        }
    }
}
