namespace Verimood.Warehouse.Application.Services.Sale.Models;

public class UpdateSaleDto
{
    public Guid? CustomerId { get; set; }
    public Guid? UserId { get; set; }
    public decimal? TotalAmount { get; set; }
}
