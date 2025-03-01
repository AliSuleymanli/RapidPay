using MediatR;

namespace RapidPay.Application.Features.CardManagement.CreateCard;

public class CreateCardCommandHandler(ICardService cardService) : IRequestHandler<CreateCardCommand, CardDto>
{
    public async Task<CardDto> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        return await cardService.CreateCardAsync(request.CreditLimit, cancellationToken);
    }
}
