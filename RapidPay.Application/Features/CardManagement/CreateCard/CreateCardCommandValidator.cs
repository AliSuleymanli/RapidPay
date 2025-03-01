using FluentValidation;

namespace RapidPay.Application.Features.CardManagement.CreateCard;

public class CreateCardCommandValidator : AbstractValidator<CreateCardCommand>
{
    public CreateCardCommandValidator()
    {
        RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0)
            .When(x => x.CreditLimit.HasValue)
            .WithMessage("Credit limit must be non-negative.");
    }
}
