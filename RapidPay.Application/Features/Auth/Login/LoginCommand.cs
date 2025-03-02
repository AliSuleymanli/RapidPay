using MediatR;

namespace RapidPay.Application.Features.Auth.Login;

public record LoginCommand(string UserName, string Password) : IRequest<LoginResultDto>;
