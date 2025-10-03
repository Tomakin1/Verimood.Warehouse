using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Verimood.Warehouse.Application.Services.Auth.Settings;
using Verimood.Warehouse.Domain.Entities;

namespace Verimood.Warehouse.Persistence.Initialization;

internal class ApplicationDbSeeder
{
    private readonly ILogger<ApplicationDbSeeder> _logger;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public ApplicationDbSeeder(UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<ApplicationDbSeeder> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task SeedDatabaseAsync(CancellationToken cancellationToken)
    {
        await SeedRolesAsync(cancellationToken); // Program çalıştığında rolleri otomatik db'ye ekler
        await SeedAdminUserAsync(); // Program çalıştığında Admin User dbye otomatik eklenir
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        var dbRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync(cancellationToken);

        foreach (var roleName in AppRoles.DefaultRoles)
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

    private async Task SeedAdminUserAsync()
    {
        var email = "admin@verimood.com";
        var userName = "adminVerimood";
        var pwd = "Admin123!";

        var adminUser = await _userManager.FindByEmailAsync(email);
        if (adminUser is null)
        {
            adminUser = new User
            {
                FirstName = $"{nameof(AppRoles.Admin)}",
                LastName = "VERIMOOD",
                Email = email,
                UserName = userName,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, pwd);
            if (!result.Succeeded)
            {
                _logger.LogWarning($"{nameof(AppRoles.Admin)} user could not be created. Errors :  {string.Join(", ", result.Errors.Select(e => e.Description))}");
                return;
            }
        }

        if (!await _userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
        {
            var roleResult = await _userManager.AddToRoleAsync(adminUser, AppRoles.Admin);
            if (!roleResult.Succeeded)
            {
                _logger.LogWarning($"{nameof(AppRoles.Admin)} user could not be added to '{AppRoles.Admin}' role.");
            }
        }
    }
}
