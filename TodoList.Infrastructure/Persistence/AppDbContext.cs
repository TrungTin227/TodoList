using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TodoList.Infrastructure.Persistence;

public sealed class AppDbContext
    : IdentityDbContext<User, Role, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

    //public DbSet<Todo> Todos => Set<Todo>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // Áp dụng Fluent API của Domain entities
        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // (Tuỳ chọn) Đổi schema + tên bảng cho Identity để sạch DB
        var schema = "auth";
        b.HasDefaultSchema(null); // giữ default cho domain, dùng schema riêng cho auth

        b.Entity<User>().ToTable("users", schema);
        b.Entity<Role>().ToTable("roles", schema);
        b.Entity<IdentityUserRole<Guid>>().ToTable("user_roles", schema);
        b.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins", schema);
        b.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens", schema);
        b.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims", schema);
        b.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims", schema);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
    
        // Giả sử bạn có một cách để lấy ID người dùng hiện tại, ví dụ:
        // var currentUserId = _userContext.GetCurrentUserId(); 

        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAtUtc = now;
                // entry.Entity.CreatedBy = currentUserId; // Gán người tạo
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.UpdatedAtUtc = now;
                // entry.Entity.UpdatedBy = currentUserId; // Gán người cập nhật
            }
        }

        return await base.SaveChangesAsync(ct);
    }
}