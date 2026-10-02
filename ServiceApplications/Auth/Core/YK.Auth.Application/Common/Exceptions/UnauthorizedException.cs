using System.Net;

namespace YK.Auth.Application.Common.Exceptions
{
    public sealed class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message) : base(
            HttpStatusCode.Unauthorized,
            "unauthorized-error",
            "Unauthorized",
            message)
        {
        }
    }
}
