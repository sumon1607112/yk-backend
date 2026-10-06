namespace YK.Auth.Application.UseCases.Account.Commands.Logout
{
    public class LogoutRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
