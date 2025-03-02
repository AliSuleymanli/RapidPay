using Microsoft.AspNetCore.Identity;
using RapidPay.Application.Features.Auth;
using RapidPay.Infrastructure.Data.Entities;
using RapidPay.Infrastructure.Repositories;

namespace RapidPay.Infrastructure.Services;

internal class UserService(IUserRepository userRepository) : IUserService
{
    private PasswordHasher<UserEntity> _passwordHasher = new PasswordHasher<UserEntity>();

    public async Task<UserDto?> ValidateUserAsync(string userName, string password, CancellationToken cancellationToken)
    {
        var userEntity = await userRepository.GetUserByUserNameAsync(userName, cancellationToken);
        if (userEntity == null)
            return null;

        var verificationResult = _passwordHasher.VerifyHashedPassword(userEntity, userEntity.PasswordHash, password);
        if (verificationResult == PasswordVerificationResult.Failed)
            return null;

        return new UserDto(userEntity.Id, userEntity.UserName, userEntity.Email);
    }
}
