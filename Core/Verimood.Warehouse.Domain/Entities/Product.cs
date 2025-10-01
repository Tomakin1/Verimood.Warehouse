﻿namespace Verimood.Warehouse.Domain.Entities;

public class Product
{
    public Guid Id { get; set; }
    public Guid CategoryId { get; set; } 
    public string Name { get; set; } = default!; 
    public string? Description { get; set; } 
    public string? Barcode { get; set; } 
    public decimal Price { get; set; } 
    public int StockQuantity { get; set; } 
    public bool IsActive { get; set; } = true; 
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Category Category { get; set; } = default!;
    public ICollection<Stock>? Stocks { get; set; } 
    public ICollection<SaleItem>? SaleItems { get; set; } 
}
