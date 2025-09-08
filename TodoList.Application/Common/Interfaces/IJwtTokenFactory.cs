using System.Security.Claims;

namespace TodoList.Application.Common.Interfaces;

public interface IJwtTokenFactory
{
    string CreateToken(Guid userId, string? email, string? userName, IEnumerable<Claim>? extraClaims = null);

}