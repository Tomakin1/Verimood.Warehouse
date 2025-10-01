namespace Verimood.Warehouse.Application.Helpers;

public class SearchPaginationRequest
{
    public string? SearchText { get; set; } 
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
