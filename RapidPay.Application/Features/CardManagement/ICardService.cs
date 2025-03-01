using RapidPay.Application.Features.CardManagement.AuthorizeCard;
using RapidPay.Application.Features.CardManagement.CreateCard;
using RapidPay.Application.Features.CardManagement.GetCardBalance;
using RapidPay.Application.Features.CardManagement.PayWithCard;
using RapidPay.Application.Features.CardManagement.UpdateCardDetails;

namespace RapidPay.Application.Features.CardManagement;

public interface ICardService
{
    Task<AuthorizationResultDto> AuthorizeCardAsync(Guid cardId, CancellationToken cancellationToken);
    Task<CardDto> CreateCardAsync(decimal? creditLimit, CancellationToken cancellationToken);
    Task<PaymentTransactionDto> PayWithCardAsync(Guid cardId, decimal paymentAmount, CancellationToken cancellationToken);
    Task<CardBalanceDto> GetCardBalanceAsync(Guid cardId, CancellationToken cancellationToken);
    Task<CardDto> UpdateCardDetailsAsync(UpdateCardDetailsCommand command, CancellationToken cancellationToken);
}
