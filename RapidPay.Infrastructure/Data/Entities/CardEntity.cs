using RapidPay.Application.Features.CardManagement;

namespace RapidPay.Infrastructure.Data.Entities;

public class CardEntity
{
    public Guid Id { get; set; }
    public string CardNumber { get; set; }
    public decimal Balance { get; set; }
    public decimal? CreditLimit { get; set; }
    public CardStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<TransactionEntity> Transactions { get; set; }
}
