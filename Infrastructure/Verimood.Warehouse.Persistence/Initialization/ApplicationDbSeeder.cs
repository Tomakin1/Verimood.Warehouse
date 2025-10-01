using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Verimood.Warehouse.Domain.Entities;

namespace Verimood.Warehouse.Persistence.Initialization;

internal class ApplicationDbSeeder
{
    private readonly ILogger<ApplicationDbSeeder> _logger;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private const string Admin = "Admin";
    private const string Employee = "Employee";
    private readonly List<string> roles = new List<string> { Admin, Employee };

    public ApplicationDbSeeder(UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<ApplicationDbSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedDatabaseAsync(CancellationToken cancellationToken)
    {
        await SeedRolesAsync(cancellationToken); // Program çalıştığında rolleri otomatik db'ye ekler
        await SeedAdminUserAsync(cancellationToken); // Program çalıştığında Admin User dbye otomatik eklenir
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        var dbRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync(cancellationToken);

        foreach (var roleName in roles)
        {
            if (!dbRoles.Contains(roleName))
            {
                var role = new Role { Name = roleName, Description = $"Default role {roleName}" };
                var result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Role '{RoleName}' could not be created.", roleName);
                }
            }
        }
    }

    private async Task SeedAdminUserAsync(CancellationToken cancellationToken)
    {
        var email = "admin@verimood.com";
        var userName = "adminVerimood";
        var pwd = "Admin123!";

        var adminUser = await _userManager.FindByEmailAsync(email);
        if (adminUser is null)
        {
            adminUser = new User
            {
                FirstName = "Admin",
                LastName = "VERIMOOD",
                Email = email,
                UserName = userName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, pwd);
            if (!result.Succeeded)
            {
                _logger.LogWarning("Admin user could not be created. Errors: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return;
            }
        }

        if (!await _userManager.IsInRoleAsync(adminUser, Admin))
        {
            var roleResult = await _userManager.AddToRoleAsync(adminUser, Admin);
            if (!roleResult.Succeeded)
            {
                _logger.LogWarning("Admin user could not be added to '{Role}' role.", Admin);
            }
        }
    }
}
