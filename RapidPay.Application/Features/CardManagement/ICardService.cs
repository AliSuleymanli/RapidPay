using RapidPay.Application.Features.CardManagement.CreateCard;

namespace RapidPay.Application.Features.CardManagement;

public interface ICardService
{
    Task<CardDto> CreateCardAsync(decimal? creditLimit, CancellationToken cancellationToken);
}
