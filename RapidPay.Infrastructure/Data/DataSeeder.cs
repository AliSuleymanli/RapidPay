using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RapidPay.Infrastructure.Data.Entities;

namespace RapidPay.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedDataAsync(RapidPayDbContext dbContext)
    {
        // Ensure database is created or migrated
        await dbContext.Database.MigrateAsync();

        await SeedUsersAsync(dbContext);
        await SeedPaymentFeesAsync(dbContext);
    }

    private static async Task SeedUsersAsync(RapidPayDbContext dbContext)
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

    private static async Task SeedPaymentFeesAsync(RapidPayDbContext dbContext)
    {
        if (!dbContext.PaymentFees.Any())
        {
            var initialFee = new PaymentFeeEntity
            {
                Id = Guid.NewGuid(),
                CurrentFee = 1.0m,
                Multiplier = 1.0m,
                UpdatedAt = DateTime.UtcNow
            };

            dbContext.PaymentFees.Add(initialFee);
            await dbContext.SaveChangesAsync();
        }
    }
}
