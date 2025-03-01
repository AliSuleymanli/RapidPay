using MediatR;

namespace RapidPay.Application.Features.CardManagement.GetCardBalance;

public class GetCardBalanceQueryHandler(ICardService cardService) : IRequestHandler<GetCardBalanceQuery, CardBalanceDto>
{
    public Task<CardBalanceDto> Handle(GetCardBalanceQuery request, CancellationToken cancellationToken)
    {
        return cardService.GetCardBalanceAsync(request.CardId, cancellationToken);
    }
}
