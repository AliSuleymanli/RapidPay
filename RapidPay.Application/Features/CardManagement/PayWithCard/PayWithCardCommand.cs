using MediatR;

namespace RapidPay.Application.Features.CardManagement.PayWithCard;

public record PayWithCardCommand(Guid CardId, decimal PaymentAmount, string IdempotencyKey) : IRequest<PaymentTransactionDto>;
