using RapidPay.Infrastructure.Data;
using RapidPay.Infrastructure.Data.Entities;

namespace RapidPay.Infrastructure.Repositories;

internal interface ICardRepository
{
    Task AddAsync(CardEntity card, CancellationToken cancellationToken);
}

internal class CardRepository(RapidPayDbContext dbContext) : ICardRepository
{
    public Task AddAsync(CardEntity card, CancellationToken cancellationToken)
    {
        dbContext.Cards.Add(card);
        return Task.CompletedTask;
    }
}
