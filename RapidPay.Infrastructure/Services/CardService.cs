using RapidPay.Application.Features.CardManagement;
using RapidPay.Application.Features.CardManagement.AuthorizeCard;
using RapidPay.Application.Features.CardManagement.CreateCard;
using RapidPay.Application.Features.CardManagement.GetCardBalance;
using RapidPay.Application.Features.CardManagement.PayWithCard;
using RapidPay.Infrastructure.Data.Entities;
using RapidPay.Infrastructure.Repositories;

namespace RapidPay.Infrastructure.Services;

internal class CardService(ICardRepository cardRepository) : ICardService
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

    public async Task<PaymentTransactionDto> PayWithCardAsync(Guid cardId, decimal paymentAmount, CancellationToken cancellationToken)
    {
        // Retrieve the card.
        var card = await cardRepository.GetByIdAsync(cardId, cancellationToken);
        if (card == null)
        {
            throw new Exception("Card not found.");
        }

        if (card.Status != "Active")
        {
            throw new Exception("Card is not active.");
        }

        // Retrieve the current fee via the repository.
        var currentFeeEntity = await cardRepository.GetLatestPaymentFeeAsync(cancellationToken);
        var fee = currentFeeEntity?.CurrentFee ?? 0m;

        var totalDeduction = paymentAmount + fee;

        if (card.Balance + (card.CreditLimit ?? 0) < totalDeduction)
        {
            throw new Exception("Insufficient funds.");
        }

        card.Balance -= totalDeduction;

        // Create a transaction record and add it via the repository.
        var transaction = new TransactionEntity
        {
            CardId = card.Id,
            Amount = paymentAmount,
            Fee = fee,
            Timestamp = DateTime.UtcNow
        };

        await cardRepository.AddTransactionAsync(transaction, cancellationToken);

        // Persist all changes.
        await cardRepository.SaveChangesAsync(cancellationToken);

        return new PaymentTransactionDto(
            transaction.Id,
            card.Id,
            paymentAmount,
            fee,
            card.Balance,
            transaction.Timestamp);
    }

    public async Task<CardBalanceDto> GetCardBalanceAsync(Guid cardId, CancellationToken cancellationToken)
    {
        // Retrieve the card from the repository.
        var card = await cardRepository.GetByIdAsync(cardId, cancellationToken);
        if (card == null)
        {
            throw new Exception("Card not found.");
        }

        decimal availableBalance = card.Balance + (card.CreditLimit ?? 0);

        return new CardBalanceDto(card.Id, card.Balance, card.CreditLimit, availableBalance);
    }
}
