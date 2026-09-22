using MediatR;
using YK.Application.Abstractions.Services;

namespace YK.Application.UseCases.Account.Register
{
    public class RegisterHandler : IRequestHandler<RegisterCommand>
    {
        private readonly IAccountService _accountService;

        public RegisterHandler(IAccountService accountService)
        {
            _accountService = accountService;
        }

        public async Task Handle(RegisterCommand request, CancellationToken cancellationToken)
        {

        }
    }
}
