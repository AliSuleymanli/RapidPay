﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RapidPay.Application.Features.CardManagement;
using RapidPay.Infrastructure.Data;
using RapidPay.Infrastructure.Repositories;
using RapidPay.Infrastructure.Services;

namespace RapidPay.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<RapidPayDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("RapidPayConnection")));

        services.AddScoped<ICardRepository, CardRepository>();
        services.AddScoped<ICardService, CardService>();

        return services;
    }
}
