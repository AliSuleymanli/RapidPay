using RapidPay.Application.Features.CardManagement;
using RapidPay.Application.Features.CardManagement.AuthorizeCard;
using RapidPay.Application.Features.CardManagement.CreateCard;
using RapidPay.Application.Features.CardManagement.GetCardBalance;
using RapidPay.Application.Features.CardManagement.PayWithCard;
using RapidPay.Application.Features.CardManagement.UpdateCardDetails;
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

    public async Task<CardDto> UpdateCardDetailsAsync(UpdateCardDetailsCommand command, CancellationToken cancellationToken)
    {
        // Retrieve the card.
        var card = await cardRepository.GetByIdAsync(command.CardId, cancellationToken);
        if (card == null)
        {
            throw new Exception("Card not found.");
        }

        // Keep a record of changes.
        var changes = new List<string>();

        if (command.NewBalance.HasValue && command.NewBalance.Value != card.Balance)
        {
            changes.Add($"Balance: {card.Balance} -> {command.NewBalance.Value}");
            card.Balance = command.NewBalance.Value;
        }

        if (command.NewCreditLimit.HasValue && command.NewCreditLimit.Value != card.CreditLimit)
        {
            changes.Add($"CreditLimit: {card.CreditLimit} -> {command.NewCreditLimit.Value}");
            card.CreditLimit = command.NewCreditLimit.Value;
        }

        if (!string.IsNullOrEmpty(command.NewStatus) && command.NewStatus != card.Status)
        {
            changes.Add($"Status: {card.Status} -> {command.NewStatus}");
            card.Status = command.NewStatus;
        }

        // Create an update log if there are changes.
        if (changes.Count > 0)
        {
            var updateLog = new CardUpdateLogEntity
            {
                CardId = card.Id,
                UpdatedAt = DateTime.UtcNow,
                Changes = string.Join("; ", changes)
            };

            await cardRepository.AddUpdateLogAsync(updateLog, cancellationToken);
        }

        await cardRepository.SaveChangesAsync(cancellationToken);

        return new CardDto(card.Id, card.CardNumber, card.Balance, card.CreditLimit, card.Status);
    }
}
