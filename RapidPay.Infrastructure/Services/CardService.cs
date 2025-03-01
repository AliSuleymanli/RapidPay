using RapidPay.Application.Features.CardManagement;
using RapidPay.Application.Features.CardManagement.CreateCard;
using RapidPay.Infrastructure.Data;
using RapidPay.Infrastructure.Data.Entities;
using RapidPay.Infrastructure.Repositories;

namespace RapidPay.Infrastructure.Services;

internal class CardService(RapidPayDbContext dbContext, ICardRepository cardRepository) : ICardService
{
    public async Task<CardDto> CreateCardAsync(decimal? creditLimit, CancellationToken cancellationToken)
    {
        var random = new Random();
        var cardNumber = string.Concat(Enumerable.Range(0, 15)
                                .Select(_ => random.Next(0, 10).ToString()));

        var cardEntity = new CardEntity
        {
            CardNumber = cardNumber,
            Balance = (decimal)(random.NextDouble() * 1000),
            CreditLimit = creditLimit,
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };

        await cardRepository.AddAsync(cardEntity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new CardDto(cardEntity.Id, cardEntity.CardNumber, cardEntity.Balance, cardEntity.CreditLimit, cardEntity.Status);
    }
}
