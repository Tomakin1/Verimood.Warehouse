using Verimood.Warehouse.Application.Services.Customer.Models;
using Verimood.Warehouse.Application.Services.User.Models;

namespace Verimood.Warehouse.Application.Services.Sale.Models;

public class GetSaleDto
{
    public Guid Id { get; set; }
    public DateTime SaleDate { get; set; }
    public decimal TotalAmount { get; set; }

    public GetUserDto User { get; set; } = default!;
    public GetCustomerDto Customer { get; set; } = default!;
}
