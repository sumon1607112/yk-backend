using YK.Application.Common.Contracts.Authentication;
using YK.Domain.Entities.Account;

namespace YK.Application.UseCases.Account.Register
{
    public class RegisterResponseDto
    {
       public User User { get; set; } = default!;
       public AuthTokensDto? Tokens { get; set; } = default!;

    }
    
}
