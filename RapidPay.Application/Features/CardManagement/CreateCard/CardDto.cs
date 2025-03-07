namespace RapidPay.Application.Features.CardManagement.CreateCard;

public record CardDto(Guid Id, string CardNumber, decimal Balance, decimal? CreditLimit, CardStatus Status);
