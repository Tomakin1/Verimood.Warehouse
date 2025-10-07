using Verimood.Warehouse.Application.Services.Product.Models;
using Verimood.Warehouse.Application.Services.User.Models;

namespace Verimood.Warehouse.Application.Services.Stock.Models;

public class GetStockDto
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    public GetUserDto User { get; set; } = default!;
    public GetProductDto Product { get; set; } = default!;

}
