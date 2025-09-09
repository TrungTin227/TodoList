using Microsoft.AspNetCore.Identity;
namespace TodoList.Infrastructure.Auth;

public sealed class Role : IdentityRole<Guid>, IAuditable, ISoftDelete
{
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedBy { get; set; }
}