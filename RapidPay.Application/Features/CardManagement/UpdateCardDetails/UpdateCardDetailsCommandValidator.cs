using FluentValidation;

namespace RapidPay.Application.Features.CardManagement.UpdateCardDetails;

public class UpdateCardDetailsCommandValidator : AbstractValidator<UpdateCardDetailsCommand>
{
    public UpdateCardDetailsCommandValidator()
    {
        RuleFor(x => x.CardId)
            .NotEqual(Guid.Empty)
            .WithMessage("CardId must be a valid Guid.");

        RuleFor(x => x.NewBalance)
            .GreaterThanOrEqualTo(0)
            .When(x => x.NewBalance.HasValue)
            .WithMessage("New balance must be non-negative.");

        RuleFor(x => x.NewCreditLimit)
            .GreaterThanOrEqualTo(0)
            .When(x => x.NewCreditLimit.HasValue)
            .WithMessage("New credit limit must be non-negative.");

        RuleFor(x => x.NewStatus)
            .NotEmpty()
            .When(x => x.NewStatus != null)
            .WithMessage("New status cannot be empty if provided.");
    }
}
