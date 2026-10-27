using Microsoft.Extensions.Options;
using PhoneNumbers;
using System.Diagnostics.CodeAnalysis;
using YK.Auth.Application.Common.Abstractions.Services.Phone;

namespace YK.Auth.Infrastructure.Phone
{
    public sealed class PhoneNumberService : IPhoneNumberService
    {
        private readonly PhoneNumberOptions _options;
        private readonly PhoneNumberUtil _phoneUtil;

        public PhoneNumberService(IOptions<PhoneNumberOptions> options, PhoneNumberUtil phoneUtil)
        {
            _options = options.Value;   // .Value is your PhoneNumberOptions, already populated from JSON
            _phoneUtil = phoneUtil;
        }

        public bool TryNormalize(string? input, [NotNullWhen(true)] out string? e164)
        {
            e164 = null;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            PhoneNumber number;

            try
            {
                number = _phoneUtil.Parse(input, _options.DefaultRegion);
            }
            catch (NumberParseException)
            {
                return false;
            }

            if (!_phoneUtil.IsValidNumber(number))
                return false;

            var region = _phoneUtil.GetRegionCodeForNumber(number);

            if (string.IsNullOrWhiteSpace(region) || !_options.AllowedRegions.Contains(region, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            var numberType = _phoneUtil.GetNumberType(number);

            if (numberType != PhoneNumberType.MOBILE && numberType != PhoneNumberType.FIXED_LINE_OR_MOBILE)
            {
                return false;
            }

            e164 = _phoneUtil.Format(number, PhoneNumberFormat.E164);

            return true;
        }
    }
}
