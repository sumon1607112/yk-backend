using YK.Application.Common.Abstractions.Persistence;
using YK.Domain.Entities.Account;

namespace YK.Application.Abstractions.Persistence
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
