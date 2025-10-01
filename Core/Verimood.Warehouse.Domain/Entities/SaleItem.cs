namespace Verimood.Warehouse.Domain.Entities;

public class SaleItem
{
    public Guid Id { get; set; }
    public Guid SaleId { get; set; }
    public Guid ProductId { get; set; }

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Sale Sale { get; set; } = default!;
    public Product Product { get; set; } = default!;
}
