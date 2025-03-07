using FluentValidation;

namespace RapidPay.Application.Features.CardManagement.AuthorizeCard;

public class AuthorizeCardCommandValidator : AbstractValidator<AuthorizeCardCommand>
{
    public AuthorizeCardCommandValidator()
    {
        RuleFor(x => x.CardId)
            .NotEqual(Guid.Empty)
            .WithMessage("CardId must be a valid Guid.");
    }
}
