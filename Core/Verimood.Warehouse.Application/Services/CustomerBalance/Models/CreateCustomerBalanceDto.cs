namespace Verimood.Warehouse.Application.Services.CustomerBalance.Models;

public class CreateCustomerBalanceDto
{
    public Guid CustomerId { get; set; }

    public decimal Balance { get; set; }

    public string? Description { get; set; }
}
