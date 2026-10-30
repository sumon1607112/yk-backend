namespace YK.Auth.Application.Common.Abstractions.Services.Phone
{
    public static class PhoneNumberServiceExtensions
    {
        public static string NormalizeOrThrow(this IPhoneNumberService phoneNumberService, string? phone, string errorMessage = "Phone number passed validation but could not be normalized.")
        {
            if (!phoneNumberService.TryNormalize(phone, out var normalizedPhone))
            {
                throw new InvalidOperationException(errorMessage);
            }

            return normalizedPhone;
        }
    }
}
