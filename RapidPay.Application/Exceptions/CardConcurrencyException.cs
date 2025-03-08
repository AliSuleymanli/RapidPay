namespace RapidPay.Application.Exceptions;

public class CardConcurrencyException : Exception
{
    public CardConcurrencyException(Guid cardId)
        : base($"Card with ID {cardId} was updated by another request.")
    {
    }
}
