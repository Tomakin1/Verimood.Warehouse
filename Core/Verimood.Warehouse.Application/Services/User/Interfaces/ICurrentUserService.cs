using System.Security.Claims;

namespace Verimood.Warehouse.Application.Services.User.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
    string FullName { get; }
    IList<string> Roles { get; }
}
