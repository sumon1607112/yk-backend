namespace YK.Auth.Application.Common.Contracts.Authentication
{
    public class AuthTokensDto
    {
        public string AccessToken { get; set; } = default!;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
