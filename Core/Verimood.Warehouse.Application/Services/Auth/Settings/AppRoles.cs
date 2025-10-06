using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Verimood.Warehouse.Application.Exceptions.Models;
using Verimood.Warehouse.Application.Services.User.Interfaces;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;

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

    public static void CheckCurrentUserIsAdmin(ICurrentUserService _currentUser)
    {
        var currentUserRoles = _currentUser.Roles;

        if (!currentUserRoles.Contains(AppRoles.Admin))
        {
            throw new UnauthorizedAccessException("Unauthorized proccess, acces denied!");
        }
    }

    public static async Task<bool> CheckUserIsAdmin(Guid Id, IReadRepository<UserNRole> readRepository, CancellationToken cancellationToken)
    {
        var userRoles = await readRepository.GetAllByFilterAsync([x => x.UserId == Id], cancellationToken, x => x.Role);

        var roleNames = new List<string>();

        foreach (var userNRole in userRoles)
        {
            var roleName = userNRole.Role.Name;
            roleNames.Add(roleName!);
        }

        if (!roleNames.Contains(AppRoles.Admin))
        {
            return false;
        }

        return true;

    }

}
