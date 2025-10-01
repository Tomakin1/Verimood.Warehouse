namespace Verimood.Warehouse.Application.Services.Role.Models;

public class GetRoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
