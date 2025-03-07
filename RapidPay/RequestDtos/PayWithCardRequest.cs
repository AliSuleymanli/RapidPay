namespace RapidPayApi.RequestDtos;

public record PayWithCardRequest(Guid CardId, decimal PaymentAmount);
