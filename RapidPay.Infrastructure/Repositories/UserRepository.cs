using Microsoft.EntityFrameworkCore;
using RapidPay.Infrastructure.Data;
using RapidPay.Infrastructure.Data.Entities;

namespace RapidPay.Infrastructure.Repositories;

internal interface IUserRepository
{
    Task<UserEntity?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken);
}

internal class UserRepository(RapidPayDbContext dbContext) : IUserRepository
{
    public Task<UserEntity?> GetUserByUserNameAsync(string userName, CancellationToken cancellationToken)
    {
        return dbContext.Users.FirstOrDefaultAsync(u => u.UserName == userName, cancellationToken);
    }
}
