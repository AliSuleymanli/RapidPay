namespace RapidPay.Application.Features.CardManagement.GetCardBalance;

public record CardBalanceDto(
        Guid CardId,
        decimal Balance,
        decimal? CreditLimit,
        decimal AvailableBalance
    );