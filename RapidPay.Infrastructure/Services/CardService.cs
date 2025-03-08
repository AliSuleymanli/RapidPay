using Microsoft.EntityFrameworkCore;
using RapidPay.Application.Exceptions;
using RapidPay.Application.Features.CardManagement;
using RapidPay.Application.Features.CardManagement.AuthorizeCard;
using RapidPay.Application.Features.CardManagement.CreateCard;
using RapidPay.Application.Features.CardManagement.GetCardBalance;
using RapidPay.Application.Features.CardManagement.PayWithCard;
using RapidPay.Application.Features.CardManagement.UpdateCardDetails;
using RapidPay.Infrastructure.Data.Entities;
using RapidPay.Infrastructure.Repositories;
using System.Text.Json;

namespace RapidPay.Infrastructure.Services;

internal class CardService(ICardRepository cardRepository, IIdempotencyRepository idempotencyRepository) : ICardService
{
    public async Task<CardDto> CreateCardAsync(decimal? creditLimit, CancellationToken cancellationToken, string idempotencyKey)
    {
        var existingRecord = await idempotencyRepository.GetRecordAsync(idempotencyKey, cancellationToken);
        if (existingRecord != null)
        {
            return JsonSerializer.Deserialize<CardDto>(existingRecord.ResponseJson)!;
        }

        var random = new Random();
        var cardNumber = string.Concat(Enumerable.Range(0, 15)
                                .Select(_ => random.Next(0, 10).ToString()));

        var cardEntity = new CardEntity
        {
            Id = Guid.NewGuid(),
            CardNumber = cardNumber,
            Balance = (decimal)(random.NextDouble() * 1000),
            CreditLimit = creditLimit,
            Status = CardStatus.Active,
            CreatedAt = DateTime.UtcNow
        };

        var resultDto = new CardDto(cardEntity.Id, cardEntity.CardNumber, cardEntity.Balance, cardEntity.CreditLimit, cardEntity.Status);

        var record = new IdempotencyRecordEntity
        {
            IdempotencyKey = idempotencyKey,
            ResponseJson = JsonSerializer.Serialize(resultDto),
            CreatedAt = DateTime.UtcNow
        };

        await idempotencyRepository.AddRecordAsync(record, cancellationToken);
        await cardRepository.AddAsync(cardEntity, cancellationToken);

        await cardRepository.SaveChangesAsync(cancellationToken);

        return resultDto;
    }

    public async Task<AuthorizationResultDto> AuthorizeCardAsync(Guid cardId, CancellationToken cancellationToken)
    {
        var card = await cardRepository.GetByIdAsync(cardId, cancellationToken);
        if (card == null)
        {
            return new AuthorizationResultDto(cardId, false, "Card not found.");
        }

        if (card.Status != CardStatus.Active)
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

    public async Task<PaymentTransactionDto> PayWithCardAsync(Guid cardId, decimal paymentAmount, CancellationToken cancellationToken, string idempotencyKey)
    {
        var existingRecord = await idempotencyRepository.GetRecordAsync(idempotencyKey, cancellationToken);
        if (existingRecord != null)
        {
            return JsonSerializer.Deserialize<PaymentTransactionDto>(existingRecord.ResponseJson)!;
        }

        // Retrieve the card.
        var card = await cardRepository.GetByIdAsync(cardId, cancellationToken);
        if (card == null)
        {
            throw new CardNotFoundException(cardId);
        }

        if (card.Status != CardStatus.Active)
        {
            throw new CardNotActiveException(card.Id);
        }

        // Retrieve the current fee via the repository.
        var currentFeeEntity = await cardRepository.GetLatestPaymentFeeAsync(cancellationToken);
        var fee = currentFeeEntity?.CurrentFee ?? 0m;

        var totalDeduction = paymentAmount + fee;

        if (card.Balance + (card.CreditLimit ?? 0) < totalDeduction)
        {
            throw new InsufficientFundsException(card.Id, card.Balance + (card.CreditLimit ?? 0), totalDeduction);
        }

        card.Balance -= totalDeduction;

        // Create a transaction record and add it via the repository.
        var transaction = new TransactionEntity
        {
            Id = Guid.NewGuid(),
            CardId = card.Id,
            Amount = paymentAmount,
            Fee = fee,
            Timestamp = DateTime.UtcNow
        };

        var resultDto = new PaymentTransactionDto(
            transaction.Id,
            card.Id,
            paymentAmount,
            fee,
            card.Balance,
            transaction.Timestamp);

        await cardRepository.AddTransactionAsync(transaction, cancellationToken);

        var record = new IdempotencyRecordEntity
        {
            IdempotencyKey = idempotencyKey,
            ResponseJson = JsonSerializer.Serialize(resultDto),
            CreatedAt = DateTime.UtcNow
        };
        await idempotencyRepository.AddRecordAsync(record, cancellationToken);

        try
        {
            await cardRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another request updated this card's rowversion.
            throw new CardConcurrencyException(card.Id);
        }

        return resultDto;
    }

    public async Task<CardBalanceDto> GetCardBalanceAsync(Guid cardId, CancellationToken cancellationToken)
    {
        // Retrieve the card from the repository.
        var card = await cardRepository.GetByIdAsync(cardId, cancellationToken);
        if (card == null)
        {
            throw new CardNotFoundException(cardId);
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
            throw new CardNotFoundException(command.CardId);
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

        if (command.NewStatus != card.Status)
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

        try
        {
            await cardRepository.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another request updated the card since we retrieved it.
            throw new CardConcurrencyException(command.CardId);
        }

        return new CardDto(card.Id, card.CardNumber, card.Balance, card.CreditLimit, card.Status);
    }
}
