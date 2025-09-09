using TodoList.Application.Auth.DTOs;
using TodoList.Application.Features.Auth.DTOs;

namespace TodoList.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    Task<AuthResultDto> GenerateToken(UserDto user, IEnumerable<string> roles, CancellationToken ct = default);
    Task<Result<AuthResultDto>> RefreshToken(string refreshToken, CancellationToken ct = default);
    Task<Result<(Guid UserId, string Email)>> ValidateRefreshToken(string refreshToken);
    
}