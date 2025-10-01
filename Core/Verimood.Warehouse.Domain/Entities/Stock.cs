namespace Verimood.Warehouse.Domain.Entities;

public class Stock
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Guid? UserId { get; set; }

    public int Quantity { get; set; }
    public string? Description { get; set; }
    public DateTime TransactionDate { get; set; } = DateTime.Now;

    public User? User { get; set; }
    public Product Product { get; set; } = default!;
}
