namespace RapidPay.Application.Features.CardManagement.PayWithCard;

public record PaymentTransactionDto(
        Guid TransactionId,
        Guid CardId,
        decimal PaymentAmount,
        decimal Fee,
        decimal NewBalance,
        DateTime Timestamp);