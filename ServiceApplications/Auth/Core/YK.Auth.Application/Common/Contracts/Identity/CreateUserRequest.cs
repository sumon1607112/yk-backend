namespace YK.Auth.Application.Common.Contracts.Identity
{
    public class CreateUserRequest
    {
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
