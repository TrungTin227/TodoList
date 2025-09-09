using TodoList.Application.Features.Auth.DTOs;

namespace TodoList.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<Result<(UserDto User, IList<string> Roles)>> AuthenticateUserAsync(string email, string password, CancellationToken ct = default);

    Task<Result<CreateUserResult>> CreateUserAsync(CreateUserRequest request, CancellationToken ct = default);

    Task<Result> AddUserToRoleAsync(Guid userId, string roleName, CancellationToken ct = default);

    Task<IList<string>> GetUserRolesAsync(Guid userId, CancellationToken ct = default);

    Task<UserDto?> GetUserByIdAsync(Guid userId, CancellationToken ct = default);

    Task<UserDto?> GetUserByEmailAsync(string email, CancellationToken ct = default);

    Task<Result> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, CancellationToken ct = default);
    
    Task<Result> UpdateUserAsync(Guid userId, string firstName, string lastName, Gender gender, CancellationToken ct = default);
}