using MediatR;

namespace RapidPay.Application.Features.CardManagement.PayWithCard;

public class PayWithCardCommandHandler(ICardService cardService) : IRequestHandler<PayWithCardCommand, PaymentTransactionDto>
{
    public async Task<PaymentTransactionDto> Handle(PayWithCardCommand request, CancellationToken cancellationToken)
    {
        return await cardService.PayWithCardAsync(request.CardId, request.PaymentAmount, cancellationToken, request.IdempotencyKey);
    }
}
