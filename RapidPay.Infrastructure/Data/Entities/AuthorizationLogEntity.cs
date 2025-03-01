namespace RapidPay.Infrastructure.Data.Entities;

public class AuthorizationLogEntity
{
    public Guid Id { get; set; }
    public Guid CardId { get; set; }
    public bool Authorized { get; set; }
    public string Notes { get; set; }
    public DateTime Timestamp { get; set; }
    public CardEntity Card { get; set; }
}
