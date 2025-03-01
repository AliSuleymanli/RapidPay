using RapidPay.Application.Features.CardManagement;
using RapidPay.Application.Features.CardManagement.AuthorizeCard;
using RapidPay.Application.Features.CardManagement.CreateCard;
using RapidPay.Infrastructure.Data.Entities;
using RapidPay.Infrastructure.Repositories;

namespace RapidPay.Infrastructure.Services;

internal class CardService(ICardRepository cardRepository) : ICardService
{
    public async Task<CardDto> CreateCardAsync(decimal? creditLimit, CancellationToken cancellationToken)
    {
        var random = new Random();
        var cardNumber = string.Concat(Enumerable.Range(0, 15).Select(_ => random.Next(0, 10).ToString()));

        var cardEntity = new CardEntity
        {
            CardNumber = cardNumber,
            Balance = (decimal)(random.NextDouble() * 1000),
            CreditLimit = creditLimit,
            Status = "Active",
            CreatedAt = DateTime.UtcNow
        };

        await cardRepository.AddAsync(cardEntity, cancellationToken);
        await cardRepository.SaveChangesAsync(cancellationToken);

        return new CardDto(cardEntity.Id, cardEntity.CardNumber, cardEntity.Balance, cardEntity.CreditLimit, cardEntity.Status);
    }

    public async Task<AuthorizationResultDto> AuthorizeCardAsync(Guid cardId, CancellationToken cancellationToken)
    {
        var card = await cardRepository.GetByIdAsync(cardId, cancellationToken);
        if (card == null)
        {
            return new AuthorizationResultDto(cardId, false, "Card not found.");
        }

        if (card.Status != "Active")
        {
            return new AuthorizationResultDto(cardId, false, "Card is not active.");
        }

        // Check for duplicate authorization attempt within 5 seconds.
        var hasDuplicate = await cardRepository.HasRecentAuthorizationAsync(cardId, TimeSpan.FromSeconds(5), cancellationToken);
        if (hasDuplicate)
        {
            return new AuthorizationResultDto(cardId, false, "Duplicate authorization attempt detected.");
        }

        var authLog = new AuthorizationLogEntity
        {
            CardId = cardId,
            Authorized = true,
            Timestamp = DateTime.UtcNow,
            Notes = "Authorization successful."
        };

        await cardRepository.AddAuthorizationLogAsync(authLog, cancellationToken);
        await cardRepository.SaveChangesAsync(cancellationToken);

        return new AuthorizationResultDto(cardId, true, "Authorization successful.");
    }
}
