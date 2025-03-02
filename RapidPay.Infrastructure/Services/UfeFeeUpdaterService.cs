using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RapidPay.Infrastructure.Data;
using RapidPay.Infrastructure.Data.Entities;

namespace RapidPay.Infrastructure.Services;

public class UfeFeeUpdaterService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<UfeFeeUpdaterService> _logger;
    private readonly Random _random;

    public UfeFeeUpdaterService(IServiceProvider serviceProvider, ILogger<UfeFeeUpdaterService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _random = new Random();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Loop until the service is stopped.
        while (!stoppingToken.IsCancellationRequested)
        {
            // Wait for 1 hour.
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<RapidPayDbContext>();

                // Retrieve the latest fee (by UpdatedAt).
                var latestFeeEntity = await dbContext.PaymentFees
                    .OrderByDescending(f => f.UpdatedAt)
                    .FirstOrDefaultAsync(stoppingToken);

                // Use a default fee of 1 if none exists.
                decimal previousFee = latestFeeEntity?.CurrentFee ?? 1m;

                // Generate a random multiplier between 0 and 2.
                decimal randomMultiplier = (decimal)_random.NextDouble() * 2m;

                decimal newFee = previousFee * randomMultiplier;

                // Create and add a new PaymentFeeEntity.
                var newFeeEntity = new PaymentFeeEntity
                {
                    CurrentFee = newFee,
                    Multiplier = randomMultiplier,
                    UpdatedAt = DateTime.UtcNow
                };

                dbContext.PaymentFees.Add(newFeeEntity);
                await dbContext.SaveChangesAsync(stoppingToken);

                _logger.LogInformation("UFE updated fee to {NewFee} with multiplier {Multiplier}", newFee, randomMultiplier);
            }
        }
    }
}