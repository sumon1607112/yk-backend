
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using YK.Application.Common.Behaviors;

namespace YK.Application
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
                cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            });

            // 2. AutoMapper Registration (Simplified with DI package)
            services.AddAutoMapper(assembly); // One line!

            // 3. Register FluentValidation
            services.AddValidatorsFromAssembly(assembly);

            return services;
        }
    }
}