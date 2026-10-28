using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using YK.Auth.Application.Common.Abstractions.Services.Identity;
using YK.Auth.Application.Common.Contracts.Identity;
using YK.Auth.Application.Common.Exceptions;
using YK.Auth.Domain.Entities.Common.Identity;
using YK.Auth.Infrastructure.Persistence;

namespace YK.Auth.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<User> _userManager;

        public IdentityService(UserManager<User> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _dbContext = context;
        }

        public async Task<bool> UserExistsAsync(string phone, string role)
        {
            var normalizedRole = _userManager.NormalizeName(role);

            return await UsersByPhoneAndRole(phone, role).AnyAsync();
        }

        public async Task CreateUserAsync(CreateUserRequest request, CancellationToken ct = default)
        {
            try
            {
                var strategy = _dbContext.Database.CreateExecutionStrategy();

                await strategy.ExecuteAsync(async () =>
                {
                    _dbContext.ChangeTracker.Clear();

                    await using var transaction = await _dbContext.Database.BeginTransactionAsync(ct);

                    var user = new User
                    {
                        PhoneNumber = request.Phone,
                        Email = request.Email,
                        UserName = AccountUserName.Create(request.Phone, request.Role)
                    };

                    var userResult = await _userManager.CreateAsync(user, request.Password);

                    if (!userResult.Succeeded)
                    {
                        var errors = userResult.Errors.ToList();

                        if (errors.Any(e => e.Code == nameof(IdentityErrorDescriber.DuplicateUserName)))
                        {
                            throw new ConflictException("A user with this phone number and role already exists.");
                        }

                        throw new BusinessRuleException(errors.Select(e => e.Description).ToList());
                    }

                    var roleResult = await _userManager.AddToRoleAsync(user, request.Role);

                    if (!roleResult.Succeeded)
                    {
                        throw new BusinessRuleException(roleResult.Errors.Select(e => e.Description).ToList());
                    }

                    await transaction.CommitAsync(ct);
                });
            }
            catch (DbUpdateException ex) when (IsUniqueViolation(ex))
            {
                throw new ConflictException("A user with this phone number and role already exists.");
            }

        }

        public async Task<User?> ValidateCredentialsAsync(string phone, string role, string password)
        {
            var normalizedRole = _userManager.NormalizeName(role);

            // 1. Find the user by phone + role
            var user = await UsersByPhoneAndRole(phone, role).FirstOrDefaultAsync();

            if (user is null)
                return null;

            // 2. Locked? Reject even if the password is correct
            if (await _userManager.IsLockedOutAsync(user))
                throw new UnauthorizedException("Account is locked. Try again later.");

            // 3. Wrong password? Count the failure
            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                await _userManager.AccessFailedAsync(user);
                return null;
            }

            // 4. Correct password: reset the failure count
            await _userManager.ResetAccessFailedCountAsync(user);
            return user;
        }


        //Helper method to get users by phone and role
        private IQueryable<User> UsersByPhoneAndRole(string phone, string role)
        {
            var normalizedRole = _userManager.NormalizeName(role);

            return from u in _dbContext.Users
                   join ur in _dbContext.UserRoles on u.Id equals ur.UserId
                   join r in _dbContext.Roles on ur.RoleId equals r.Id
                   where u.PhoneNumber == phone && r.NormalizedName == normalizedRole
                   select u;
        }

        private static bool IsUniqueViolation(DbUpdateException ex) => ex.InnerException is SqlException { Number: 2601 or 2627 };
    }
}
