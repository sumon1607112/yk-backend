using System;
using System.Collections.Generic;
using System.Text;
using YK.Application.Abstractions.Persistence;
using YK.Domain.Entities.Users;
using YK.Infrastructure.Abstractions.Persistence.Contexts;
using YK.Infrastructure.Common.Abstractions.Persistence;

namespace YK.Infrastructure.Abstractions.Persistence.Repositories
{
    public class UserRepository: Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
