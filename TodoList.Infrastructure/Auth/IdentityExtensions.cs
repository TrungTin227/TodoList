using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
namespace TodoList.Infrastructure.Auth;
public static class IdentityExtensions
{
    public static IdentityBuilder AddIdentityInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        var identityBuilder = services.AddIdentityCore<User>(o =>
            {
                o.User.RequireUniqueEmail = true;
                o.Password.RequireDigit = false;
                o.Password.RequireNonAlphanumeric = false;
                o.Password.RequiredLength = 6;
            })
            .AddRoles<Role>()
            .AddEntityFrameworkStores<AppDbContext>();
        return identityBuilder;
    }
}