namespace Verimood.Warehouse.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string? TaxNumber { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public decimal TotalBalance { get; set; }  // customer balance kaydı buraya yazılacak borç ise -ye düşecek borç ödenince manuel total balance güncellenecek

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Sale>? Sales { get; set; }
    public ICollection<CustomerBalance>? CustomerBalances { get; set; }
}
