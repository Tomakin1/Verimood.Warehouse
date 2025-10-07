namespace Verimood.Warehouse.Application.Services.Customer.Models;

public class CreateCustomerDto
{
    public string Name { get; set; } = default!;
    public string? TaxNumber { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public decimal TotalBalance { get; set; }
}
