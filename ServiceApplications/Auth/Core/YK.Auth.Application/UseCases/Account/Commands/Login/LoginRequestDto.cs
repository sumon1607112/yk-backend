namespace YK.Auth.Application.UseCases.Account.Commands.Login
{
    public class LoginRequestDto
    {
        public string Phone { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
