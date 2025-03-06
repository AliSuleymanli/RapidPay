using Microsoft.EntityFrameworkCore;
using RapidPay.Infrastructure.Data;
using RapidPay.Infrastructure.Data.Entities;

namespace RapidPay.Infrastructure.Repositories;

internal interface IIdempotencyRepository
{
    Task<IdempotencyRecordEntity?> GetRecordAsync(string idempotencyKey, CancellationToken cancellationToken);
    Task AddRecordAsync(IdempotencyRecordEntity record, CancellationToken cancellationToken);
}

internal class IdempotencyRepository : IIdempotencyRepository
{
    private readonly RapidPayDbContext _dbContext;

    public IdempotencyRepository(RapidPayDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<IdempotencyRecordEntity?> GetRecordAsync(string idempotencyKey, CancellationToken cancellationToken)
    {
        return _dbContext.IdempotencyRecords
            .FirstOrDefaultAsync(r => r.IdempotencyKey == idempotencyKey, cancellationToken);
    }

    public Task AddRecordAsync(IdempotencyRecordEntity record, CancellationToken cancellationToken)
    {
        _dbContext.IdempotencyRecords.Add(record);
        return Task.CompletedTask;
    }
}