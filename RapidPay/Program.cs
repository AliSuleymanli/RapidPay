using RapidPay.Application;
using RapidPay.Infrastructure;
using RapidPay.Infrastructure.Data;
using RapidPayApi.Auth;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.



builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Seed test data.
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<RapidPayDbContext>();
    await RapidPay.Infrastructure.Data.DataSeeder.SeedDataAsync(dbContext);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
