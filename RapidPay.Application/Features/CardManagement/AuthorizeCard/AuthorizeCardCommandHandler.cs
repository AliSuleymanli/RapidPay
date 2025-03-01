using MediatR;

namespace RapidPay.Application.Features.CardManagement.AuthorizeCard;

public class AuthorizeCardCommandHandler : IRequestHandler<AuthorizeCardCommand, AuthorizationResultDto>
{
    private readonly ICardService _cardService;

    public AuthorizeCardCommandHandler(ICardService cardService)
    {
        _cardService = cardService;
    }

    public async Task<AuthorizationResultDto> Handle(AuthorizeCardCommand request, CancellationToken cancellationToken)
    {
        return await _cardService.AuthorizeCardAsync(request.CardId, cancellationToken);
    }
}
