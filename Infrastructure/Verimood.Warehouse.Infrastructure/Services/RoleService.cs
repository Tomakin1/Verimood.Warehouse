using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Role.Interfaces;
using Verimood.Warehouse.Application.Services.Role.Models;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;

namespace Verimood.Warehouse.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly IReadRepository<Role> _roleReadRepository;
    private readonly IWriteRepository<Role> _roleWriteRepository;
    private readonly RoleManager<Role> _roleManager;

    public RoleService(IReadRepository<Role> roleReadRepository, IWriteRepository<Role> roleWriteRepository, RoleManager<Role> roleManager)
    {
        _roleReadRepository = roleReadRepository;
        _roleWriteRepository = roleWriteRepository;
        _roleManager = roleManager;
    }

    public async Task<BaseResponse<Guid>> CreateAsync(CreateRoleDto dto, CancellationToken cancellationToken)
    {
        var role = new Role
        {
            Name = dto.Name,
            Description = dto.Description,

        };

        var result = await _roleWriteRepository.CreateAsync(role, cancellationToken);
        if (!result)
        {
            return BaseResponse<Guid>.ErrorResponse("Ann error ocurred while creating role", 500);
        }

        return BaseResponse<Guid>.SuccessResponse(role.Id, 201, "Role created successfully");
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await _roleReadRepository.GetByIdAsync(id, cancellationToken);
        if (role is null)
        {
            return BaseResponse<object>.ErrorResponse("Role not found", 404);
        }

        var result = await _roleWriteRepository.DeleteAsync(role, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error ocurred while deleting role", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Role deleted successfully");
    }

    public async Task<BaseResponse<List<GetRoleDto>>> GetAllAsync(CancellationToken cancellationToken)
    {
        var roles = await _roleManager.Roles.ToListAsync(cancellationToken);

        if (roles is null || !roles.Any())
        {
            return BaseResponse<List<GetRoleDto>>.ErrorResponse("No roles found", 404);
        }

        return BaseResponse<List<GetRoleDto>>.SuccessResponse(roles.Select(MapToGetRoleDto).ToList(), 200, $"{roles.Count} roles found");
    }

    public async Task<BaseResponse<GetRoleDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var role = await _roleReadRepository.GetByIdAsync(id, cancellationToken);
        if (role is null)
        {
            return BaseResponse<GetRoleDto>.ErrorResponse("Role not found", 404);
        }

        return BaseResponse<GetRoleDto>.SuccessResponse(MapToGetRoleDto(role), 200, "Role found");
    }

    public async Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateRoleDto dto, CancellationToken cancellationToken)
    {
        var role = await _roleReadRepository.GetByIdAsync(Id, cancellationToken);
        if (role is null)
        {
            return BaseResponse<object>.ErrorResponse("Role not found", 404);
        }

        if (!string.IsNullOrEmpty(dto.Name))
            role.Name = dto.Name;

        if (!string.IsNullOrEmpty(dto.Description))
            role.Description = dto.Description;

        var result = await _roleWriteRepository.UpdateAsync(role, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error ocurred while updating role", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Role updated successfully");
    }

    private GetRoleDto MapToGetRoleDto(Role role)
    {
        return new GetRoleDto
        {
            Id = role.Id,
            Name = role.Name!,
            Description = role.Description
        };
    }
}
