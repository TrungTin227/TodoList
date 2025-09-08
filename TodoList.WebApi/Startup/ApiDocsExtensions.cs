using Scalar.AspNetCore;
namespace TodoList.WebApi.Startup;

public static class ApiDocsExtensions
{
    public static IServiceCollection AddApiDocs(this IServiceCollection services)
    {
        services.AddOpenApi();
        return services;
    }

    public static WebApplication UseApiDocs(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();

            app.MapScalarApiReference(options =>
            {
                options
                    .WithTitle("TodoList API")
                    .WithDarkMode(true)
                    // khớp route OpenAPI ở trên (mặc định /openapi/{documentName}.json)
                    .WithOpenApiRoutePattern("/openapi/{documentName}.json");
            });
        }
        return app;
    }
}