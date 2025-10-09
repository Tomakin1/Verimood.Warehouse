namespace Verimood.Warehouse.Application.Services.Role.Models;

public class CreateRoleDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
