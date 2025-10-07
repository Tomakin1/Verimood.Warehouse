using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Product.Models;

namespace Verimood.Warehouse.Application.Services.Product.Interfaces;

public interface IProductService
{
    Task<BaseResponse<Guid>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateProductDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<GetProductDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<PaginationResponse<GetProductDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken);
}
