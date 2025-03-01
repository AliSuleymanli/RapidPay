using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace RapidPay.Infrastructure.Data;

public class RapidPayDbContextFactory : IDesignTimeDbContextFactory<RapidPayDbContext>
{
    public RapidPayDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<RapidPayDbContext>();
        var connectionString = configuration.GetConnectionString("RapidPayConnection");
        builder.UseSqlServer(connectionString);
        return new RapidPayDbContext(builder.Options);
    }
}
