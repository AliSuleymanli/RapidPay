using MediatR;

namespace RapidPay.Application.Features.Auth.Login;

public class LoginCommandHandler(IUserService userService, IJwtTokenGenerator tokenGenerator) : IRequestHandler<LoginCommand, LoginResultDto>
{
    public async Task<LoginResultDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userService.ValidateUserAsync(request.UserName, request.Password, cancellationToken);
        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = tokenGenerator.GenerateToken(user);
        return new LoginResultDto(token, user);
    }
}
