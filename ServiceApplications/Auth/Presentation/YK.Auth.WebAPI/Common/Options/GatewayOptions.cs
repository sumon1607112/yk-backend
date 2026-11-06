namespace YK.Auth.WebAPI.Common.Options
{
    public sealed class GatewayOptions
    {
        public const string SectionName = "Gateway";
        public string SharedSecret { get; init; } = string.Empty;
    }
}
