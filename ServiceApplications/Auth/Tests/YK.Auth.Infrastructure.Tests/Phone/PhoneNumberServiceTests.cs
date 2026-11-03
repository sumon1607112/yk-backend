using Microsoft.Extensions.Options;
using PhoneNumbers;
using YK.Auth.Infrastructure.Phone;

namespace YK.Auth.Infrastructure.Tests.Phone
{
    public class PhoneNumberServiceTests
    {

        [Theory]
        [InlineData("01712345678", "+8801712345678")]
        [InlineData("+8801712345678", "+8801712345678")]
        [InlineData("8801712345678", "+8801712345678")]
        [InlineData("+880 1712-345678", "+8801712345678")]
        public void TryNormalize_ValidBdMobile_ReturnsE164(string input, string expected)
        {
            var sut = CreateSut();

            var result = sut.TryNormalize(input, out var e164);

            Assert.True(result);
            Assert.Equal(expected, e164);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData(" ")]
        [InlineData("1234567890")]
        [InlineData("+919876543210")]
        [InlineData("02-9561234")]
        [InlineData("abc")]
        public void TryNormalize_InvalidInput_ReturnsFalse(string? input)
        {
            var sut = CreateSut();

            var result = sut.TryNormalize(input, out var e164);

            Assert.False(result);
            Assert.Null(e164);
        }

        //Helper method to create a PhoneNumberService instance with default options

        private static PhoneNumberService CreateSut() => new(Options.Create(new PhoneNumberOptions
        {
            DefaultRegion = "BD",
            AllowedRegions = ["BD"]
        }), PhoneNumberUtil.GetInstance());
    }
}
