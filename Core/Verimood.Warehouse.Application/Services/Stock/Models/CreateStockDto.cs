namespace Verimood.Warehouse.Application.Services.Stock.Models;

public class CreateStockDto
{
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
    public string? Description { get; set; }
}
