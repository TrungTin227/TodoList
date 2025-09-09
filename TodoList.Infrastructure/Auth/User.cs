using Microsoft.AspNetCore.Identity;

namespace TodoList.Infrastructure.Auth;

public sealed class User : IdentityUser<Guid>, IAuditable, ISoftDelete
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName => $"{LastName} {FirstName}".Trim();
    public Gender Gender { get; set; }
    public bool IsFirstLogin { get; set; } = true;

    // Audit
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedBy { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedBy { get; set; }
}