namespace Verimood.Warehouse.Application.Services.Product.Models;

public class UpdateProductDto
{
    public Guid? CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Barcode { get; set; }
    public decimal? Price { get; set; }
    public int? StockQuantity { get; set; }
    public bool? IsActive { get; set; } 
}
