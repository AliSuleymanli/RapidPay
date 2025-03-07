using FluentValidation;

namespace RapidPay.Application.Features.CardManagement.PayWithCard;

public class PayWithCardCommandValidator : AbstractValidator<PayWithCardCommand>
{
    public PayWithCardCommandValidator()
    {
        RuleFor(x => x.CardId)
            .NotEqual(Guid.Empty)
            .WithMessage("CardId must be a valid Guid.");

        RuleFor(x => x.PaymentAmount)
            .GreaterThan(0)
            .WithMessage("Payment amount must be greater than zero.");

        RuleFor(x => x.IdempotencyKey)
            .NotEmpty()
            .WithMessage("An idempotency key is required for payment processing.");
    }
}
