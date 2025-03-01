using MediatR;
using RapidPay.Application.Features.CardManagement.CreateCard;

namespace RapidPay.Application.Features.CardManagement.UpdateCardDetails;

public record UpdateCardDetailsCommand(
        Guid CardId,
        decimal? NewBalance,
        decimal? NewCreditLimit,
        string? NewStatus) : IRequest<CardDto>;
