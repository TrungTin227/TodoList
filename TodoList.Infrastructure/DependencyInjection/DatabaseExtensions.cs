using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TodoList.Infrastructure.DependencyInjection;

public static class DatabaseExtensions
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration cfg)
    {
        var connStr = cfg.GetConnectionString("Default")
                      ?? throw new InvalidOperationException("Missing ConnectionStrings:Default");

        services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(
                    connStr,
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)
                )
            // .EnableSensitiveDataLogging(appEnv.IsDevelopment())  // tuỳ chọn
            // .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking) // tuỳ chọn
        );

        return services;
    }
}