namespace RapidPay.Application.Features.Auth.Login;

public record LoginResultDto(string Token, UserDto User);