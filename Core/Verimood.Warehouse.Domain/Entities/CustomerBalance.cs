namespace Verimood.Warehouse.Domain.Entities;

public class CustomerBalance
{
    public Guid Id { get; set; } 
    public Guid CustomerId { get; set; } 

    public decimal Debit { get; set; } = 0; 
    public decimal Credit { get; set; } = 0;    
    public decimal Balance => Credit - Debit; 

    public string? Description { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Customer Customer { get; set; } = default!;


}
