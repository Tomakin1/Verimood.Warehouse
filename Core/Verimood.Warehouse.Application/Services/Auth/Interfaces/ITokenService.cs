using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Auth.Models;

namespace Verimood.Warehouse.Application.Services.Auth.Interfaces;

public interface ITokenService
{
    JwtSecurityToken GenerateAccessToken(Domain.Entities.User user, IList<string> roles);
    string GenerateRefreshToken();
    Task<BaseResponse<RefreshTokenRequest>> RefreshAccesToken(RefreshTokenRequest request, CancellationToken cancellationToken);
    ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token);
}
