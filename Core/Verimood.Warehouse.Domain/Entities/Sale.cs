namespace Verimood.Warehouse.Domain.Entities;

public class Sale
{
    public Guid Id { get; set; } 
    public Guid CustomerId { get; set; } 
    public Guid UserId { get; set; } 

    public DateTime SaleDate { get; set; } = DateTime.UtcNow; 
    public decimal TotalAmount { get; set; }

    public User User { get; set; } = default!; 
    public Customer Customer { get; set; } = default!;
    public ICollection<SaleItem>? SaleItems { get; set; } 

}
