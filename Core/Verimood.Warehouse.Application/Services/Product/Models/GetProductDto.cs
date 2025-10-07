using Verimood.Warehouse.Application.Services.Category.Models;

namespace Verimood.Warehouse.Application.Services.Product.Models;

public class GetProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string? Barcode { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public GetCategoryDto Category { get; set; }
}
