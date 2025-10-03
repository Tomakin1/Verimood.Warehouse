namespace Verimood.Warehouse.Application.Services.User.Models;

public class ChangePasswordDto
{
    public string NewPassword { get; set; } = default!;
    public string OldPassword { get; set; } = default!;
}
