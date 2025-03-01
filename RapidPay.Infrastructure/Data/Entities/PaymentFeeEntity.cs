namespace RapidPay.Infrastructure.Data.Entities;

public class PaymentFeeEntity
{
    public Guid Id { get; set; }
    public decimal CurrentFee { get; set; }
    public decimal Multiplier { get; set; }
    public DateTime UpdatedAt { get; set; }
}
