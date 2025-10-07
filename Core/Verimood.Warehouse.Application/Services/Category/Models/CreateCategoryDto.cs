namespace Verimood.Warehouse.Application.Services.Category.Models;

public class CreateCategoryDto
{
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
}
