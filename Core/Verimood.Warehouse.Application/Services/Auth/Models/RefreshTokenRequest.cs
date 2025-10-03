namespace Verimood.Warehouse.Application.Services.Auth.Models;

public class RefreshTokenRequest
{
    public string AccesToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}
