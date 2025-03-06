namespace RapidPay.Infrastructure.Data.Entities;

public class IdempotencyRecordEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string IdempotencyKey { get; set; } = string.Empty;
    // Store the response as JSON.
    public string ResponseJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
