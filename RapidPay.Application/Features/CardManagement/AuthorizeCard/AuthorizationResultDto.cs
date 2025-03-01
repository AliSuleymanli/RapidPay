namespace RapidPay.Application.Features.CardManagement.AuthorizeCard;

public record AuthorizationResultDto(Guid CardId, bool Authorized, string Message);