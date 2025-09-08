using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TodoList.Infrastructure.DesignTime;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var cfg = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", true)
            .AddJsonFile($"appsettings.{env}.json", true)
            .AddEnvironmentVariables()
            .Build();

        var connStr = cfg.GetConnectionString("Default")
                      ?? throw new InvalidOperationException("Missing ConnectionStrings:Default");

        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connStr, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));

        return new AppDbContext(opts.Options);
    }
}