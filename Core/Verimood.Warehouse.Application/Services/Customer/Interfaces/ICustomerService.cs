using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Customer.Models;

namespace Verimood.Warehouse.Application.Services.Customer.Interfaces;

public interface ICustomerService
{
    Task<BaseResponse<Guid>> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateCustomerDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<GetCustomerDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken);
    Task<BaseResponse<PaginationResponse<GetCustomerDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken);
   
}