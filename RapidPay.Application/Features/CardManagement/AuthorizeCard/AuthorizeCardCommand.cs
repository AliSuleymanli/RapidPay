using MediatR;

namespace RapidPay.Application.Features.CardManagement.AuthorizeCard;

public record AuthorizeCardCommand(Guid CardId) : IRequest<AuthorizationResultDto>;
