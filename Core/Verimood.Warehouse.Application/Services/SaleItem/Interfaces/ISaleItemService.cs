using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Sale.Models;
using Verimood.Warehouse.Application.Services.SaleItem.Models;

namespace Verimood.Warehouse.Application.Services.SaleItem.Interfaces;

public interface ISaleItemService
{
    Task<BaseResponse<List<Guid>>> CreateBulkAsync(List<CreateSaleItemDto> dtos, CancellationToken cancellationToken);
    Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateSaleItemDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> DeleteAsync(List<Guid> Ids, CancellationToken cancellationToken);
    Task<BaseResponse<GetSaleItemDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<PaginationResponse<GetSaleItemDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken);
    Task<BaseResponse<PaginationResponse<GetSaleItemDto>>> GetBySaleId(Guid Id, PaginationRequest request, CancellationToken cancellationToken);
}


