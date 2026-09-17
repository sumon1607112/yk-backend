using System;
using System.Collections.Generic;
using System.Text;
using YK.Domain.Entities.Account;

namespace YK.Application.Common.Abstractions.Services.Identity
{
    public interface IIdentityService
    {
        Task<bool> UserExistsAsync(User user);
    }
}
