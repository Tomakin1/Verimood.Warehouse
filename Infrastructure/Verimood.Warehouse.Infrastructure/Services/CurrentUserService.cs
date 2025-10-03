using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Verimood.Warehouse.Application.Services.User.Interfaces;

namespace Verimood.Warehouse.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(
        IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public Guid UserId =>
        IsAuthenticated && Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)
            ? id
            : throw new UnauthorizedAccessException("User can not authanticated!");

    public string Email =>
        IsAuthenticated ? User.FindFirstValue(ClaimTypes.Email) ?? throw new UnauthorizedAccessException("Email not found!")
                        : throw new UnauthorizedAccessException("User can not authanticated!");




    public string FullName
    {
        get
        {
            var username = User?.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
            return !string.IsNullOrEmpty(username)
                ? username
                : null;
        }
    }

    public IList<string> Roles =>
        _httpContextAccessor.HttpContext?.User?
            .FindAll(ClaimTypes.Role)
            .Select(c => c.Value)
            .ToList()
        ?? new List<string>();
}
