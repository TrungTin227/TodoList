namespace TodoList.Domain.Common;

public abstract class AuditableEntity
{
    public Guid Id { get; set; }

    // --- Thông tin tạo ---
    public DateTime CreatedAtUtc { get; set; }
    public Guid? CreatedBy { get; set; } // Nullable, có thể hệ thống tự tạo

    // --- Thông tin cập nhật ---
    public DateTime? UpdatedAtUtc { get; set; } // Nullable, vì khi mới tạo chưa có cập nhật
    public Guid? UpdatedBy { get; set; } // Nullable

    // --- Thông tin xóa mềm (Soft Delete) ---
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; } // Nullable
    public Guid? DeletedBy { get; set; } // Nullable
}