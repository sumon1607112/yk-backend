using System.Diagnostics.CodeAnalysis;

namespace YK.Auth.Application.Common.Abstractions.Services.Phone
{
    public interface IPhoneNumberService
    {
        bool TryNormalize(string? input, [NotNullWhen(true)] out string? e164);
    }
}
