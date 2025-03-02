namespace RapidPay.Application.Features.Auth;

public interface IJwtTokenGenerator
{
    string GenerateToken(UserDto user);
}
