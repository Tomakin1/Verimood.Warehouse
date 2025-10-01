using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Role.Models;
using Verimood.Warehouse.Application.Services.User.Models;

namespace Verimood.Warehouse.Application.Services.User.Interfaces;

public interface IUserService
{
    Task<BaseResponse<object>> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken); // Admin tarafından depoya yeni kullanıcı ekleme
    Task<BaseResponse<object>> UpdateAsync(UpdateUserDto dto, CancellationToken cancellationToken); // Admin tarafından çalışan güncelleme
    Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken); // Admin tarafından çalışan silme
    Task<BaseResponse<GetUserDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken); // çalışan bilgisi getirme
    Task<BaseResponse<PaginationResponse<GetUserDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken); // Sistemdeki tüm çalışanları paginate ile getirme
    Task<BaseResponse<object>> ActivateAsync(Guid Id, CancellationToken cancellationToken); // Admin tarafından kullanıcı active etme
    Task<BaseResponse<object>> DeactivateAsync(Guid Id, CancellationToken cancellationToken); // Admin tarafından kullanıcı deactive etme
    Task<BaseResponse<List<GetRoleDto>>> GetRolesAsync(Guid Id, CancellationToken cancellationToken); // Sistemdeki çalışanın rollerini getirme
}
