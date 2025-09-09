var builder = WebApplication.CreateBuilder(args);

// --- 1. Cấu hình đăng ký dịch vụ (Dependency Injection) ---

// Lấy các đối tượng cần thiết để truyền vào các phương thức mở rộng
var services = builder.Services;
var configuration = builder.Configuration;
services.AddHttpContextAccessor();
// Đăng ký dịch vụ theo từng tầng một cách rõ ràng
services
    .AddApplication() // Đăng ký dịch vụ của tầng Application (MediatR, AutoMapper, Validators...)
    .AddInfrastructureServices(configuration)// Đăng ký TOÀN BỘ dịch vụ của tầng Infrastructure
    .AddIdentityAndAuthentication(builder.Configuration)
    .AddPresentation(); // Đăng ký dịch vụ của tầng Presentation/API (Controllers, API Docs, CORS...)
services.AddApiDocs();


// --- 2. Xây dựng ứng dụng ---
var app = builder.Build();


// --- 3. Cấu hình HTTP Request Pipeline (Middleware) ---

// Sử dụng phương thức mở rộng của tầng Presentation để cấu hình pipeline
// Điều này giúp đóng gói logic nhưng vẫn giữ được sự rõ ràng
app.UsePresentation(); 
app.UseApiDocs();

app.Run();