namespace RapidPay.Infrastructure.Data.Entities;

public class TransactionEntity
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public decimal Amount { get; set; }
    public decimal Fee { get; set; }
    public DateTime Timestamp { get; set; }

    public CardEntity Card { get; set; }
}
