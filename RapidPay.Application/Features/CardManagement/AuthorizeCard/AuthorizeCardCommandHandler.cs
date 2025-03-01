using MediatR;

namespace RapidPay.Application.Features.CardManagement.AuthorizeCard;

public class AuthorizeCardCommandHandler(ICardService cardService) : IRequestHandler<AuthorizeCardCommand, AuthorizationResultDto>
{
    public async Task<AuthorizationResultDto> Handle(AuthorizeCardCommand request, CancellationToken cancellationToken)
    {
        return await cardService.AuthorizeCardAsync(request.CardId, cancellationToken);
    }
}
