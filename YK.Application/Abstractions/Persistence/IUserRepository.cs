using YK.Application.Common.Abstractions.Persistence;
using YK.Domain.Entities.Users;

namespace YK.Application.Abstractions.Persistence
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
