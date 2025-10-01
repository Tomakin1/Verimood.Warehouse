using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Role.Models;
using Verimood.Warehouse.Application.Services.User.Interfaces;
using Verimood.Warehouse.Application.Services.User.Models;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;

namespace Verimood.Warehouse.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IWriteRepository<User> _userWriteRepository;
    private readonly IReadRepository<User> _userReadRepository;

    public UserService(IWriteRepository<User> userWriteRepository, IReadRepository<User> userReadRepository)
    {
        _userWriteRepository = userWriteRepository;
        _userReadRepository = userReadRepository;
    }

    public async Task<BaseResponse<object>> ActivateAsync(Guid id, CancellationToken cancellationToken)
    {
        var user = await _userReadRepository.GetByIdAsync(id, cancellationToken);

        if (user is null)
        {
            return BaseResponse<object>.ErrorResponse("User not found", 404);
        }

        user.IsActive = true;

        var result = await _userWriteRepository.UpdateAsync(user, cancellationToken);

        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error occurred while updating the user", 500);
        }

        return BaseResponse<object>.SuccessResponse(204, "User activated successfully");
    }

    public Task<BaseResponse<object>> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<object>> DeactivateAsync(Guid Id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<PaginationResponse<GetUserDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<GetUserDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<List<GetRoleDto>>> GetRolesAsync(Guid Id, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task<BaseResponse<object>> UpdateAsync(UpdateUserDto dto, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
