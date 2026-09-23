using YK.Auth.Application.Common.Abstractions.Persistence;
using YK.Auth.Domain.Entities.Common;

namespace YK.Auth.Application.Abstractions.Persistence
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
