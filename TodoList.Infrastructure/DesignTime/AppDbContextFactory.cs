using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace TodoList.Infrastructure.DesignTime;

public sealed class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        // Tự dò nơi có appsettings.json (ưu tiên WebApi)
        var cwd = Directory.GetCurrentDirectory();
        var candidates = new[]
        {
            cwd,
            Path.Combine(cwd, "..", "TodoList.WebApi"),
            Path.Combine(cwd, "..", "..", "TodoList.WebApi"),
        };
        var basePath = candidates.FirstOrDefault(p => File.Exists(Path.Combine(p, "appsettings.json"))) ?? cwd;

        var cfg = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.developer.json", optional: true, reloadOnChange: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables() // cho phép override: ConnectionStrings__Default
            .Build();

        var conn = cfg.GetConnectionString("Default")
                   ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default")
                   ?? throw new InvalidOperationException("Missing ConnectionStrings:Default");

        var opts = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(conn, b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName))
            .Options;

        return new AppDbContext(opts);
    }
}