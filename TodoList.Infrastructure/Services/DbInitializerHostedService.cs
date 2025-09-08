using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace TodoList.Infrastructure.Services;

public sealed class DbInitializerHostedService : IHostedService
{
    private readonly IServiceProvider _sp;
    private readonly IHostEnvironment _env;
    public DbInitializerHostedService(IServiceProvider sp, IHostEnvironment env) => (_sp, _env) = (sp, env);

    public async Task StartAsync(CancellationToken ct)
    {
        // Gợi ý: chỉ auto-migrate ở DEV
        if (!_env.IsDevelopment()) return;

        using var scope = _sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.MigrateAsync(ct);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;
}