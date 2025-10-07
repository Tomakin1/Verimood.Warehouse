namespace Verimood.Warehouse.Domain.Entities;

public class CustomerBalance
{
    public Guid Id { get; set; } 
    public Guid CustomerId { get; set; }

    public decimal Balance { get; set; } // Satıştan para alındıysa + Borç ile verildiyse -

    public string? Description { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Customer Customer { get; set; } = default!;


}
