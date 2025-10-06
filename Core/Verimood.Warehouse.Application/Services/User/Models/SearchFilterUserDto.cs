namespace Verimood.Warehouse.Application.Services.User.Models;

public class SearchFilterUserDto
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool? IsActive { get; set; }
    public string? SearchTerm { get; set; }

}
