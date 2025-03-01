using Microsoft.EntityFrameworkCore;
using RapidPay.Infrastructure.Data.Entities;

namespace RapidPay.Infrastructure.Data;

public class RapidPayDbContext : DbContext
{
    public RapidPayDbContext(DbContextOptions<RapidPayDbContext> options)
            : base(options)
    {
    }

    public DbSet<CardEntity> Cards { get; set; }
    public DbSet<TransactionEntity> Transactions { get; set; }
    public DbSet<AuthorizationLogEntity> AuthorizationLogs { get; set; }
    public DbSet<PaymentFeeEntity> PaymentFees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Fluent API configurations can be added here if needed.
    }
}
