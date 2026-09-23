using System.Net;

namespace YK.Application.Common.Exceptions
{
    public sealed class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(
            HttpStatusCode.NotFound,
            "not-found-error",
            "Resource Not Found",
            message)
        {
        }
    }
}
