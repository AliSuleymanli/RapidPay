using RapidPay.Application.Features.CardManagement.AuthorizeCard;
using RapidPay.Application.Features.CardManagement.CreateCard;

namespace RapidPay.Application.Features.CardManagement;

public interface ICardService
{
    Task<AuthorizationResultDto> AuthorizeCardAsync(Guid cardId, CancellationToken cancellationToken);
    Task<CardDto> CreateCardAsync(decimal? creditLimit, CancellationToken cancellationToken);
}
