﻿using Microsoft.EntityFrameworkCore;
using RapidPay.Infrastructure.Data;
using RapidPay.Infrastructure.Data.Entities;

namespace RapidPay.Infrastructure.Repositories;

internal interface ICardRepository
{
    Task AddAsync(CardEntity card, CancellationToken cancellationToken);
    Task<CardEntity?> GetByIdAsync(Guid cardId, CancellationToken cancellationToken);
    Task<bool> HasRecentAuthorizationAsync(Guid cardId, TimeSpan window, CancellationToken cancellationToken);
    Task AddAuthorizationLogAsync(AuthorizationLogEntity authLog, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}


internal class CardRepository(RapidPayDbContext dbContext) : ICardRepository
{
    public Task AddAsync(CardEntity card, CancellationToken cancellationToken)
    {
        dbContext.Cards.Add(card);
        return Task.CompletedTask;
    }

    public Task<CardEntity?> GetByIdAsync(Guid cardId, CancellationToken cancellationToken)
    {
        return dbContext.Cards.FirstOrDefaultAsync(c => c.Id == cardId, cancellationToken);
    }

    public Task<bool> HasRecentAuthorizationAsync(Guid cardId, TimeSpan window, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        return dbContext.AuthorizationLogs.AnyAsync(
            log => log.CardId == cardId && (now - log.Timestamp) < window,
            cancellationToken);
    }

    public Task AddAuthorizationLogAsync(AuthorizationLogEntity authLog, CancellationToken cancellationToken)
    {
        dbContext.AuthorizationLogs.Add(authLog);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}

