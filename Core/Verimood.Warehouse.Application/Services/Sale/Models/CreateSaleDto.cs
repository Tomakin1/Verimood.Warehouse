using Verimood.Warehouse.Application.Services.SaleItem.Models;

namespace Verimood.Warehouse.Application.Services.Sale.Models;

public class CreateSaleDto
{
    public Guid CustomerId { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }

    public List<CreateSaleItemDto> SaleItems { get; set; }

}
