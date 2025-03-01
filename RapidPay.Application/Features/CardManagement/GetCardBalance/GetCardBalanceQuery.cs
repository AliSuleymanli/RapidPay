using MediatR;

namespace RapidPay.Application.Features.CardManagement.GetCardBalance;

public record GetCardBalanceQuery(Guid CardId) : IRequest<CardBalanceDto>;
