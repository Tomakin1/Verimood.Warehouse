namespace Verimood.Warehouse.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? TaxNumber { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<Sale>? Sales { get; set; }
    public ICollection<CustomerBalance>? CustomerBalances { get; set; }
}
