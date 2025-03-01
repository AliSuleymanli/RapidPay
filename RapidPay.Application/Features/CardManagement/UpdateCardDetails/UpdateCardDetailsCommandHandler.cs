using MediatR;
using RapidPay.Application.Features.CardManagement.CreateCard;

namespace RapidPay.Application.Features.CardManagement.UpdateCardDetails;

public class UpdateCardDetailsCommandHandler(ICardService cardService) : IRequestHandler<UpdateCardDetailsCommand, CardDto>
{
    public async Task<CardDto> Handle(UpdateCardDetailsCommand request, CancellationToken cancellationToken)
    {
        return await cardService.UpdateCardDetailsAsync(request, cancellationToken);
    }
}
