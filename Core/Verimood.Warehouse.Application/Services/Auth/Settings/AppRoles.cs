using System.Collections.ObjectModel;
using Verimood.Warehouse.Application.Services.User.Interfaces;

namespace Verimood.Warehouse.Application.Services.Auth.Settings;

public class AppRoles
{

    public const string Admin = nameof(Admin);
    public const string Employee = nameof(Employee);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>(new[]
    {
        Admin,
        Employee
    });

    public static void CheckUserIsAdmin(ICurrentUserService _currentUser)
    {
        var currentUserRoles = _currentUser.Roles;

        if (!currentUserRoles.Contains(AppRoles.Admin))
        {
            throw new UnauthorizedAccessException("Unauthorized proccess, acces denied!");
        }
    }


}
