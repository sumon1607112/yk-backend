
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using YK.Auth.Application.Common.Behaviors;

namespace YK.Auth.Application
{
    // Add 'static' here 
    public static class DependencyInjection
    {
        // Now the extension method will work
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            // 1. Register MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(assembly);
                cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
            });

            // 2. AutoMapper Registration (Simplified with DI package)
            services.AddAutoMapper(cfg => cfg.AddMaps(assembly)); // One line!

            // 3. Register FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}