namespace YK.Auth.Infrastructure.Phone
{
    public class PhoneNumberOptions
    {
        public const string SectionName = "PhoneNumber";

        public string DefaultRegion { get; set; } = "BD";
        public string[] AllowedRegions { get; set; } = [];
    }
}
