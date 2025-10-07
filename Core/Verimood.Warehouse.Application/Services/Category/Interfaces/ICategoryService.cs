using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Category.Models;
using Verimood.Warehouse.Application.Services.Product.Models;

namespace Verimood.Warehouse.Application.Services.Category.Interfaces;

public interface ICategoryService
{
    Task<BaseResponse<Guid>> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateCategoryDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<GetCategoryDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<PaginationResponse<GetCategoryDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken);
    Task<BaseResponse<PaginationResponse<GetProductDto>>> GetCategoryProductsAsync(Guid Id,PaginationRequest request, CancellationToken cancellationToken);

}
