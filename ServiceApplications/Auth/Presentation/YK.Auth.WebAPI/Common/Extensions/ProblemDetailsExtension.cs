namespace YK.Auth.WebAPI.Common.Extensions
{
    public static class ProblemDetailsExtension
    {
        public static IServiceCollection AddApiProblemDetails(this IServiceCollection services)
        {
            services.AddProblemDetails(options =>
            {
                options.CustomizeProblemDetails = context =>
                {
                    context.ProblemDetails.Extensions["traceId"] =
                        context.HttpContext.TraceIdentifier;
                };
            });

            return services;
        }
    }
}
