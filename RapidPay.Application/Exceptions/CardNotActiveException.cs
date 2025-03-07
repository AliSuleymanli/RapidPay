namespace RapidPay.Application.Exceptions;

public class CardNotActiveException : Exception
{
    public CardNotActiveException(Guid cardId)
        : base($"Card with id {cardId} is not active.")
    {
    }
}
