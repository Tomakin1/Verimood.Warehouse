namespace Verimood.Warehouse.Application.Services.User.Models;

public class UpdateUserDto
{
    public string? FirstName { get; set; } = default!;
    public string? LastName { get; set; } = default!;
    public string? Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
}
