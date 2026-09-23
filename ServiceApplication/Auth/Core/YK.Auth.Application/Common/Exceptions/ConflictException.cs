using System.Net;

namespace YK.Auth.Application.Common.Exceptions
{
    public sealed class ConflictException : AppException
    {
        public ConflictException(string message) : base(
            HttpStatusCode.Conflict,
            "conflict-error",
            "Conflict Found",
            message)
        {
        }
    }
}
