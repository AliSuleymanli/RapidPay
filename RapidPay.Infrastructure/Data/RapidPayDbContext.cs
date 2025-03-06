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
    public DbSet<CardUpdateLogEntity> CardUpdateLogs { get; set; }
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<IdempotencyRecordEntity> IdempotencyRecords { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}
