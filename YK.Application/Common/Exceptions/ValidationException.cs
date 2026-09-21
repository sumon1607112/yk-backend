using System.Net;

namespace YK.Application.Common.Exceptions
{
    public sealed class ValidationException : AppException
    {
        public IReadOnlyDictionary<string, string[]> Errors { get; }

        public ValidationException(
            IReadOnlyDictionary<string, string[]> errors)
            : base(
                HttpStatusCode.BadRequest,
                "validation-error",
                "Validation Failed",
                "One or more validation errors occurred.")
        {
            Errors = errors;
        }
    }
}
