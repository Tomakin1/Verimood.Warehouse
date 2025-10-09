using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Sale.Models;

namespace Verimood.Warehouse.Application.Services.Sale.Interfaces;

public interface ISaleService
{
    Task<BaseResponse<Guid>> CreateAsync(CreateSaleDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateSaleDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<GetSaleDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<PaginationResponse<GetSaleDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken);
    Task<BaseResponse<PaginationResponse<GetSaleDto>>> GetByCustomerId(Guid CustomerId, PaginationRequest request, CancellationToken cancellationToken);
}
