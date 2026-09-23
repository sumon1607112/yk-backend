using System.Net;

namespace YK.Application.Common.Exceptions
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
