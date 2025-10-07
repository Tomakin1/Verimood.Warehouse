using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Stock.Models;

namespace Verimood.Warehouse.Application.Services.Stock.Interfaces;

public interface IStockService
{
    Task<BaseResponse<Guid>> CreateAsync(CreateStockDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateStockDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<GetStockDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<PaginationResponse<GetStockDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken);
    Task<BaseResponse<PaginationResponse<GetStockDto>>> GetByProductId(Guid productId, PaginationRequest request, CancellationToken cancellationToken);
}
