namespace TodoList.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Guid> CreateUserAsync(string username, string password, string? displayName, CancellationToken ct);
    Task<(Guid userId, string email, string? userName, string? displayName)?> GetUserByEmailAsync(string email, CancellationToken ct);
    Task<bool> CheckPasswordAsync(string email, string password, CancellationToken ct);
    
}