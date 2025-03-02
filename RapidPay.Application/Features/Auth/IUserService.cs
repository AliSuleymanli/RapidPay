namespace RapidPay.Application.Features.Auth;

public interface IUserService
{
    Task<UserDto?> ValidateUserAsync(string userName, string password, CancellationToken cancellationToken);
}
