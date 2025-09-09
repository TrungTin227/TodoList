using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace TodoList.Infrastructure.Persistence;

public sealed class AppDbContext
    : IdentityDbContext<User, Role, Guid>
{
    private readonly ICurrentUserService? _currentUser;
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
    
    // Runtime ctor (DI sẽ gọi ctor này)
    public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserService currentUser)
        : base(options) => _currentUser = currentUser;
    //public DbSet<Todo> Todos => Set<Todo>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

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
        b.Entity<RefreshToken>(e =>
        {
            e.ToTable("refresh_tokens", "auth");          // cùng schema 'auth'
            e.HasKey(x => x.Id);
            e.HasIndex(x => x.Token).IsUnique();          // tra cứu nhanh theo token
            e.HasIndex(x => x.UserId);                    // lọc theo user

            // 64 bytes → Base64 ~ 88 ký tự
            e.Property(x => x.Token).IsRequired().HasMaxLength(88).IsUnicode(false);

            // IPv4/IPv6 tối đa 45 ký tự
            e.Property(x => x.CreatedByIp).HasMaxLength(45).IsUnicode(false);
            e.Property(x => x.RevokedByIp).HasMaxLength(45).IsUnicode(false);

            e.Property(x => x.Created).IsRequired();
            e.Property(x => x.Expires).IsRequired();

            e.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
        b.Entity<RefreshToken>().HasQueryFilter(rt => !rt.User.IsDeleted);

        // Filter soft-delete cho mọi entity implement ISoftDelete
        foreach (var et in b.Model.GetEntityTypes())
        {
            if (typeof(ISoftDelete).IsAssignableFrom(et.ClrType))
            {
                var param = Expression.Parameter(et.ClrType, "e");
                var prop  = Expression.Property(param, nameof(ISoftDelete.IsDeleted));
                var body  = Expression.Equal(prop, Expression.Constant(false));
                var lambda = Expression.Lambda(body, param);
                b.Entity(et.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var uid = _currentUser?.UserId;

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is IAuditable a)
            {
                if (entry.State == EntityState.Added)
                {
                    a.CreatedAtUtc = now;
                    a.CreatedBy = uid;
                }
                else if (entry.State == EntityState.Modified)
                {
                    a.UpdatedAtUtc = now;
                    a.UpdatedBy = uid;
                }
            }

            if (entry.State == EntityState.Deleted && entry.Entity is ISoftDelete s)
            {
                entry.State = EntityState.Modified;
                s.IsDeleted = true;
                s.DeletedAtUtc = now;
                s.DeletedBy = uid;
            }
        }
        return await base.SaveChangesAsync(ct);
    }
}