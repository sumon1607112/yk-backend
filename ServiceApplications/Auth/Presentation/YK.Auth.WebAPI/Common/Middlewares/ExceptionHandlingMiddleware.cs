using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using YK.Auth.Application.Common.Exceptions;

namespace YK.Auth.WebAPI.Common.Middlewares
{
    public sealed class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IProblemDetailsService problemDetailsService)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                // Client disconnected: nothing to return, not an error
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", context.TraceIdentifier);

                await HandleExceptionAsync(context, problemDetailsService, exception);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, IProblemDetailsService problemDetailsService, Exception exception)
        {
            var problemDetails = exception switch
            {
                ValidationException validationException => CreateValidationProblem(context, validationException),

                BusinessRuleException businessRuleException =>
                    CreateBusinessRuleProblem(context, businessRuleException),

                AppException appException =>
                    CreateApplicationProblem(context, appException),

                _ =>
                    CreateInternalServerProblem(context)
            };

            context.Response.StatusCode = problemDetails.Status!.Value;

            await problemDetailsService.WriteAsync(
                new ProblemDetailsContext
                {
                    HttpContext = context,
                    ProblemDetails = problemDetails
                });
        }

        private static ProblemDetails CreateValidationProblem(HttpContext context, FluentValidation.ValidationException exception)
        {
            var errors = exception.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(x => x.ErrorMessage)
                        .Distinct()
                        .ToArray());

            var problemDetails = new ProblemDetails
            {
                Type = "validation-error",
                Title = "Validation Failed",
                Status = StatusCodes.Status400BadRequest,
                Detail = "One or more validation errors occurred.",
                Instance = context.Request.Path
            };

            problemDetails.Extensions["errors"] = errors;

            return problemDetails;
        }

        private static ProblemDetails CreateBusinessRuleProblem(HttpContext context, BusinessRuleException exception)
        {
            var problemDetails = new ProblemDetails
            {
                Type = exception.Type,
                Title = exception.Title,
                Status = (int)exception.StatusCode,
                Detail = exception.Message,
                Instance = context.Request.Path
            };

            problemDetails.Extensions["errors"] = exception.Errors;

            return problemDetails;
        }

        private static ProblemDetails CreateApplicationProblem(HttpContext context, AppException exception)
        {
            return new ProblemDetails
            {
                Type = exception.Type,
                Title = exception.Title,
                Status = (int)exception.StatusCode,
                Detail = exception.Message,
                Instance = context.Request.Path
            };
        }

        private static ProblemDetails CreateInternalServerProblem(
            HttpContext context)
        {
            return new ProblemDetails
            {
                Type = "internal-server-error",
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = "An unexpected error occurred.",
                Instance = context.Request.Path
            };
        }

    }
}
