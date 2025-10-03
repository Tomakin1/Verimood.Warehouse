using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Verimood.Warehouse.Application.Exceptions.Models;
using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Auth.Interfaces;
using Verimood.Warehouse.Application.Services.Auth.Models;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Infrastructure.Settings.JWT;

namespace Verimood.Warehouse.Infrastructure.Services;

public class TokenService : ITokenService
{
    private readonly UserManager<User> _userManager;
    private readonly JwtSettings _jwtSettings;

    public TokenService(UserManager<User> userManager, IOptions<JwtSettings> jwtSettings)
    {
        _userManager = userManager;
        _jwtSettings = jwtSettings.Value;
    }

    public JwtSecurityToken GenerateAccessToken(User user, IList<string> roles)
    {
        List<Claim> claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.FirstName + " " + user.LastName),
            new Claim(JwtRegisteredClaimNames.Email, user.Email)
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));

        var token = new JwtSecurityToken(

            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.TokenExpirationInMinutes > 0 ? _jwtSettings.TokenExpirationInMinutes : 960),
            claims: claims,
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)

        );

        return token;
    }

    public string GenerateRefreshToken()
    {
        byte[] randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string? token)
    {
        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)),
            ValidateLifetime = false
        };

        var handler = new JwtSecurityTokenHandler();
        var principal = handler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

        if (validatedToken is not JwtSecurityToken jwt ||
            !jwt.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
        {
            throw new SecurityTokenException("Invalid token format.");
        }

        return principal;
    }

    public async Task<BaseResponse<RefreshTokenRequest>> RefreshAccesToken(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        var principal = GetPrincipalFromExpiredToken(request.AccesToken)!;
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await _userManager.FindByIdAsync(userId!);

        if (user is null)
            throw new UnauthorizedException("Informations doesn't exist");

        if (user.RefreshToken != request.RefreshToken)
            throw new UnauthorizedException("Invalid refresh token");

        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
            throw new UnauthorizedException("Please login again");

        var roles = await _userManager.GetRolesAsync(user);
        var claims = await _userManager.GetClaimsAsync(user);

        var newAccessToken = GenerateAccessToken(user, roles);
        var newRefreshToken = GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(30);
        await _userManager.UpdateAsync(user);

        return BaseResponse<RefreshTokenRequest>.SuccessResponse(new RefreshTokenRequest
        {
            AccesToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
            RefreshToken = newRefreshToken!
        });

    }
}
