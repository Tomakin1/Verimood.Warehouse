using Verimood.Warehouse.Application.Services.Product.Models;

namespace Verimood.Warehouse.Application.Services.SaleItem.Models;

public class GetSaleItemDto
{
    public Guid Id { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
    public DateTime CreatedAt { get; set; }

    public GetProductDto Product { get; set; } = default!;

}
