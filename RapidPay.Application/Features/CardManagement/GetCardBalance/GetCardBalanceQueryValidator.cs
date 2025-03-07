using FluentValidation;

namespace RapidPay.Application.Features.CardManagement.GetCardBalance;

public class GetCardBalanceQueryValidator : AbstractValidator<GetCardBalanceQuery>
{
    public GetCardBalanceQueryValidator()
    {
        RuleFor(x => x.CardId)
            .NotEqual(Guid.Empty)
            .WithMessage("CardId must be a valid Guid.");
    }
}
