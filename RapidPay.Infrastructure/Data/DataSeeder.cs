using Microsoft.AspNetCore.Identity;
using RapidPay.Infrastructure.Data.Entities;

namespace RapidPay.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(RapidPayDbContext dbContext)
    {
        if (!dbContext.Users.Any())
        {
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                UserName = "testuser",
                Email = "testuser@example.com"
            };

            var hasher = new PasswordHasher<UserEntity>();
            user.PasswordHash = hasher.HashPassword(user, "Test@123");

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();
        }
    }
}
