using Verimood.Warehouse.Application.Services.Customer.Models;

namespace Verimood.Warehouse.Application.Services.CustomerBalance.Models;

public class GetCustomerBalanceDto
{
    public Guid Id { get; set; }

    public decimal Balance { get; set; } // Satıştan para alındıysa + Borç ile verildiyse -

    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public GetCustomerDto Customer { get; set; } = default!;
}
