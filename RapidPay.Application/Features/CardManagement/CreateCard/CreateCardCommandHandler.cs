using MediatR;

namespace RapidPay.Application.Features.CardManagement.CreateCard;

public class CreateCardCommandHandler : IRequestHandler<CreateCardCommand, CardDto>
{
    private readonly ICardService _cardService;

    public CreateCardCommandHandler(ICardService cardService)
    {
        _cardService = cardService;
    }

    public async Task<CardDto> Handle(CreateCardCommand request, CancellationToken cancellationToken)
    {
        return await _cardService.CreateCardAsync(request.CreditLimit, cancellationToken);
    }
}
