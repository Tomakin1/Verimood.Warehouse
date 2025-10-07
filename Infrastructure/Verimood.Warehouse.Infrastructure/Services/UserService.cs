using Microsoft.AspNetCore.Identity;
using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Auth.Settings;
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
    private readonly ICurrentUserService _currentUser;
    private readonly UserManager<User> _userManager;
    private readonly IReadRepository<UserNRole> _userNRoleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IReadRepository<UserNRole> _userNRoleReadRepository;

    public UserService(
        IWriteRepository<User> userWriteRepository,
        IReadRepository<User> userReadRepository,
        ICurrentUserService currentUser,
        UserManager<User> userManager,
        IReadRepository<UserNRole> userNRoleRepository,
        IUserRepository userRepository,
        IReadRepository<UserNRole> userNRoleReadRepository

        )
    {
        _userWriteRepository = userWriteRepository;
        _userReadRepository = userReadRepository;
        _currentUser = currentUser;
        _userManager = userManager;
        _userNRoleRepository = userNRoleRepository;
        _userRepository = userRepository;
        _userNRoleReadRepository = userNRoleReadRepository;
        
    }

    public async Task<BaseResponse<object>> ActivateAsync(Guid Id, CancellationToken cancellationToken)
    {

        var user = await _userReadRepository.GetByIdAsync(Id, cancellationToken);

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

        return BaseResponse<object>.SuccessResponse(null, 204, null);
    }

    public async Task<BaseResponse<Guid>> CreateAsync(CreateUserDto dto, CancellationToken cancellationToken)
    {

        var existingUser = await _userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
            return BaseResponse<Guid>.ErrorResponse("User already exist", 409);

        var user = new User
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            UserName = dto.Email,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRolesAsync(user, new[] { AppRoles.Employee });
            return BaseResponse<Guid>.SuccessResponse(user.Id, 201, "User Created Successfully");
        }
        else
        {
            return BaseResponse<Guid>.ErrorResponse($"Ann error ocurred while creating user {string.Join(", ", result.Errors.Select(e => e.Description))}", 500);
        }
    }

    public async Task<BaseResponse<object>> DeactivateAsync(Guid Id, CancellationToken cancellationToken)
    {

        var user = await _userReadRepository.GetByIdAsync(Id, cancellationToken);

        if (user is null)
        {
            return BaseResponse<object>.ErrorResponse("User not found", 404);
        }

        user.IsActive = false;

        var result = await _userWriteRepository.UpdateAsync(user, cancellationToken);
        if (!result)
            return BaseResponse<object>.ErrorResponse("An error ocurred while updating user", 500);

        return BaseResponse<object>.SuccessResponse(null, 204, null);

    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken)
    {

        var user = await _userReadRepository.GetByIdAsync(Id, cancellationToken);

        if (user is null)
        {
            return BaseResponse<object>.ErrorResponse("User not found", 404);
        }

        var result = await _userWriteRepository.DeleteAsync(user, cancellationToken);
        if (!result)
            return BaseResponse<object>.ErrorResponse("An error ocurred while Deleting user", 500);

        return BaseResponse<object>.SuccessResponse(null, 204, null);
    }

    public async Task<BaseResponse<PaginationResponse<GetUserDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _userReadRepository.GetAllPaginatedAsync(
            cancellationToken,
            null,
            x => x.OrderByDescending(x => x.CreatedAt),
            request.Page,
            request.PageSize,
            null
        );

        var users = response.entities;
        var totalCount = response.totalCount;

        if (users is null || !users.Any())
        {
            return BaseResponse<PaginationResponse<GetUserDto>>.SuccessResponse(new PaginationResponse<GetUserDto>
            {
                Items = new List<GetUserDto>(),
                Page = request.Page,
                PageSize = request.PageSize,
                TotalCount = totalCount
            }, 200, "Users not found");
        }

        return BaseResponse<PaginationResponse<GetUserDto>>.SuccessResponse(new PaginationResponse<GetUserDto>
        {
            Items = users.Select(MapToGetUserDto).ToList(),
            Page = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount
        }, 200, null);
    }

    public async Task<BaseResponse<GetUserDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {

        var user = await _userReadRepository.GetByIdAsync(Id, cancellationToken);

        if (user is null)
        {
            return BaseResponse<GetUserDto>.ErrorResponse("User not found", 404);
        }


        return BaseResponse<GetUserDto>.SuccessResponse(MapToGetUserDto(user), 200, null);
    }

    public async Task<BaseResponse<List<GetRoleDto>>> GetRolesAsync(Guid Id, CancellationToken cancellationToken)
    {
        var user = await _userReadRepository.GetByIdAsync(Id, cancellationToken);

        if (user is null)
        {
            return BaseResponse<List<GetRoleDto>>.ErrorResponse("User not found", 404);
        }

        var userRoles = await _userNRoleRepository.GetAllByFilterAsync([x => x.UserId == Id], cancellationToken, x => x.Role);

        if (userRoles is null || !userRoles.Any())
        {
            return BaseResponse<List<GetRoleDto>>.ErrorResponse("User roles not found", 404);
        }

        return BaseResponse<List<GetRoleDto>>.SuccessResponse(userRoles.Select(x => new GetRoleDto
        {
            Id = x.RoleId,
            Name = x.Role.Name!,
            Description = x.Role.Description

        }).ToList(), 200);
    }

    public async Task<BaseResponse<PaginationResponse<GetUserDto>>> SearchAsync(SearchFilterUserDto dto, CancellationToken cancellationToken)
    {
        var response = await _userRepository.SearchAsync(
            dto.Page,
            dto.PageSize,
            cancellationToken,
            dto.SearchTerm,
            dto.IsActive
            );

        var totalCount = response.totalCount;
        var users = response.users;

        if (users is null || !users.Any())
        {
            return BaseResponse<PaginationResponse<GetUserDto>>.SuccessResponse(new PaginationResponse<GetUserDto>
            {
                Items = new List<GetUserDto>(),
                Page = dto.Page,
                PageSize = dto.PageSize,
                TotalCount = totalCount
            }, 200, "Users Not found");
        }

        return BaseResponse<PaginationResponse<GetUserDto>>.SuccessResponse(new PaginationResponse<GetUserDto>
        {
            Items = users.Select(MapToGetUserDto).ToList(),
            Page = dto.Page,
            PageSize = dto.PageSize,
            TotalCount = totalCount
        }, 200, $"{totalCount} users found");


    }

    public async Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateUserDto dto, CancellationToken cancellationToken)
    {
        var user = await _userReadRepository.GetByIdAsync(Id, cancellationToken);

        if (user is null)
        {
            return BaseResponse<object>.ErrorResponse("User not found", 404);
        }

        var isAdmin = await AppRoles.CheckUserIsAdmin(Id, _userNRoleReadRepository, cancellationToken);

        if (isAdmin)
        {
            return BaseResponse<object>.ErrorResponse("You can not update admin user", 403);
        }



        if (!string.IsNullOrEmpty(dto.FirstName))
        {
            user.FirstName = dto.FirstName;
        }
        if (!string.IsNullOrEmpty(dto.LastName))
        {
            user.LastName = dto.LastName;
        }
        if (!string.IsNullOrEmpty(dto.Email))
        {
            user.Email = dto.Email;
        }
        if (!string.IsNullOrEmpty(dto.PhoneNumber))
        {
            user.PhoneNumber = dto.PhoneNumber;
        }

        var result = await _userWriteRepository.UpdateAsync(user, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error ocurred while updating user", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, null);
    }


    private GetUserDto MapToGetUserDto(User user)
    {
        return new GetUserDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber
        };
    }
}
