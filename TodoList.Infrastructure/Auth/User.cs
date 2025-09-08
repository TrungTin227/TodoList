using Microsoft.AspNetCore.Identity;
namespace TodoList.Infrastructure.Auth;

public sealed  class User : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName => $"{LastName} {FirstName}".Trim();
    public Gender Gender { get; set; }
    public bool IsFirstLogin { get; set; } = true; // Mặc định là true, sẽ được set thành false khi người dùng đăng nhập lần đầu tiên
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Lưu Guid của User đã tạo
    public Guid CreatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    // Lưu Guid của User đã cập nhật
    public Guid UpdatedBy { get; set; }

    public bool IsDeleted { get; set; }

}