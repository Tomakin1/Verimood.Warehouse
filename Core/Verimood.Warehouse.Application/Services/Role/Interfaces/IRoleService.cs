using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Role.Models;

namespace Verimood.Warehouse.Application.Services.Role.Interfaces;

public interface IRoleService
{
    Task<BaseResponse<Guid>> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateRoleDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> DeleteAsync(Guid id, CancellationToken cancellationToken);
    Task<BaseResponse<GetRoleDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<BaseResponse<List<GetRoleDto>>> GetAllAsync(CancellationToken cancellationToken);
}
