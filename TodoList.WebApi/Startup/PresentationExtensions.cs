using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Diagnostics;
using TodoList.WebApi.Contracts;

namespace TodoList.WebApi.Startup;

public static class PresentationExtensions
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddControllers()
                // Nếu bạn dùng FluentValidation, thường tắt filter mặc định của MVC:
                .ConfigureApiBehaviorOptions(o => o.SuppressModelStateInvalidFilter = true);

        services.ConfigureHttpJsonOptions(o =>
        {
            o.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            o.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        });
        

        return services;
    }

    public static WebApplication UsePresentation(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            // Redirect "/" -> "/scalar"
            app.MapGet("/", () => TypedResults.Redirect("/scalar"))
                .ExcludeFromDescription();
        }
        else
        {
            app.UseGlobalExceptionHandler();
            // app.MapGet("/", () => TypedResults.Redirect("/scalar")).ExcludeFromDescription(); // nếu muốn prod cũng redirect
        }
        
        app.UseHttpsRedirection();   
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        return app;
    }

    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(builder =>
        {
            builder.Run(async ctx =>
            {
                var _ = ctx.Features.Get<IExceptionHandlerPathFeature>()?.Error;

                ctx.Response.ContentType = "application/json";
                ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;

                var payload = new ApiResult<object>(
                    IsSuccess: false,
                    Message: "Unexpected server error",
                    Data: null,
                    ErrorCode: "server_error");

                await ctx.Response.WriteAsJsonAsync(payload);
            });
        });
        return app;
    }
}
