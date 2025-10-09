﻿namespace Verimood.Warehouse.Domain.Entities;

public class Category
{
    public Guid Id { get; set; } 
    public string Name { get; set; } = default!; 
    public string? Description { get; set; } 
    public bool IsActive { get; set; } = true; 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Product>? Products { get; set; }
}
