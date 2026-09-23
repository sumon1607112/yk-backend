using YK.Application.Common.Abstractions.Persistence;
using YK.Domain.Entities.Common;

namespace YK.Application.Abstractions.Persistence
{
    public interface IUserRepository : IRepository<User>
    {
    }
}
