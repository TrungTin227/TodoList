using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddDatabase(builder.Configuration)
    .AddApiDocs();

builder.Services.AddHostedService<DbInitializerHostedService>();
// 1. Gọi phương thức mở rộng từ lớp Infrastructure
builder.Services.AddIdentityInfrastructure(builder.Configuration)
    // 2. Bổ sung các dịch vụ chỉ dành cho web tại đây
    .AddSignInManager()
    .AddDefaultTokenProviders();

var app = builder.Build();
app.UsePresentation();   // ExceptionHandler + AuthZ + Controllers + Swagger (dev)
app.UseApiDocs();
app.Run();