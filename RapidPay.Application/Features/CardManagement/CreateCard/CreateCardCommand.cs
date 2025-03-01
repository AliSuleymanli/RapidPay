using MediatR;

namespace RapidPay.Application.Features.CardManagement.CreateCard;

public record CreateCardCommand(decimal? CreditLimit) : IRequest<CardDto>;
