using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using RapidPay.Application.Behaviours;
using RapidPay.Application.Features.CardManagement.CreateCard;

namespace RapidPay.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<CreateCardCommandValidator>();
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddMediatR(config =>
            config.RegisterServicesFromAssembly(typeof(CreateCardCommandHandler).Assembly)
        );

        return services;
    }
}
