namespace RapidPay.Infrastructure.Data.Entities;

public class CardUpdateLogEntity
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string Changes { get; set; } = string.Empty;
}
