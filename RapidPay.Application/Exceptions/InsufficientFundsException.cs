namespace RapidPay.Application.Exceptions;

public class InsufficientFundsException : Exception
{
    public InsufficientFundsException(Guid cardId, decimal currentBalance, decimal requiredAmount)
        : base($"Card with id {cardId} has insufficient funds. Current balance: {currentBalance}, Required: {requiredAmount}.")
    {
    }
}
