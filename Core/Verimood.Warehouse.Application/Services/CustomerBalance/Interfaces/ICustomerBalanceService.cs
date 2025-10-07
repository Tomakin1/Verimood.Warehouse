using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.CustomerBalance.Models;

namespace Verimood.Warehouse.Application.Services.CustomerBalance.Interfaces;

public interface ICustomerBalanceService
{
    Task<BaseResponse<Guid>> CreateAsync(CreateCustomerBalanceDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateCustomerBalanceDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<GetCustomerBalanceDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<PaginationResponse<GetCustomerBalanceDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken);
    Task<BaseResponse<PaginationResponse<GetCustomerBalanceDto>>> GetByCustomerIdAsync(Guid customerId, PaginationRequest request, CancellationToken cancellationToken);
}
