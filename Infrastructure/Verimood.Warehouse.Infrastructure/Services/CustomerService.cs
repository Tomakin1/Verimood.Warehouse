using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Customer.Interfaces;
using Verimood.Warehouse.Application.Services.Customer.Models;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;

namespace Verimood.Warehouse.Infrastructure.Services;

public class CustomerService : ICustomerService
{
    private readonly IReadRepository<Customer> _customerReadRepository;
    private readonly IWriteRepository<Customer> _customerWriteRepository;

    public CustomerService(IReadRepository<Customer> customerReadRepository, IWriteRepository<Customer> customerWriteRepository)
    {
        _customerReadRepository = customerReadRepository;
        _customerWriteRepository = customerWriteRepository;
    }

    public async Task<BaseResponse<Guid>> CreateAsync(CreateCustomerDto dto, CancellationToken cancellationToken)
    {
        var existCustomer = await _customerReadRepository.GetByFilterAsync([x => x.Email == dto.Email], cancellationToken);

        if (existCustomer is not null)
            return BaseResponse<Guid>.ErrorResponse("Customer with the same email already exists.", 400);

        var customer = new Customer
        {
            Name = dto.Name,
            Email = dto.Email,
            PhoneNumber = dto.PhoneNumber,
            Address = dto.Address,
            TaxNumber = dto.TaxNumber,
            TotalBalance = dto.TotalBalance
        };

        var result = await _customerWriteRepository.CreateAsync(customer, cancellationToken);
        if (!result)
        {
            return BaseResponse<Guid>.ErrorResponse("An error occurred while creating customer", 500);
        }

        return BaseResponse<Guid>.SuccessResponse(customer.Id, 201, "Customer created successfully");
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken)
    {
        var customer = await _customerReadRepository.GetByIdAsync(Id, cancellationToken);
        if (customer is null)
        {
            return BaseResponse<object>.ErrorResponse("Customer not found", 404);
        }

        var result = await _customerWriteRepository.DeleteAsync(customer, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error occurred while deleting the customer.", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Customer deleted successfully.");
    }

    public async Task<BaseResponse<PaginationResponse<GetCustomerDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _customerReadRepository.GetAllPaginatedAsync(
                   cancellationToken,
                   null,
                   x => x.OrderByDescending(x => x.CreatedAt),
                   request.Page,
                   request.PageSize,
                   null);

        var customers = response.entities;
        var totalCount = response.totalCount;

        if (customers is null || !customers.Any())
        {
            return BaseResponse<PaginationResponse<GetCustomerDto>>.ErrorResponse("No customers found.", 404);
        }

        return BaseResponse<PaginationResponse<GetCustomerDto>>.SuccessResponse(
            new PaginationResponse<GetCustomerDto>
            {
                Items = customers.Select(MapToGetCustomerDto).ToList(),
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page
            }, 200,
            $"{totalCount} category found");
    }

    public async Task<BaseResponse<GetCustomerDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        var customer = await _customerReadRepository.GetByIdAsync(Id, cancellationToken);
        if (customer is null)
        {
            return BaseResponse<GetCustomerDto>.ErrorResponse("Customer not found", 404);
        }

        return BaseResponse<GetCustomerDto>.SuccessResponse(MapToGetCustomerDto(customer), 200, "Customer retrieved successfully");
    }

    public async Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateCustomerDto dto, CancellationToken cancellationToken)
    {
        var customer = await _customerReadRepository.GetByIdAsync(Id, cancellationToken);

        if (customer is null)
        {
            return BaseResponse<object>.ErrorResponse("Customer not found", 404);
        }

        if (!string.IsNullOrEmpty(dto.Name))
            customer.Name = dto.Name;

        if (!string.IsNullOrEmpty(dto.Email))
            customer.Email = dto.Email;

        if (!string.IsNullOrEmpty(dto.PhoneNumber))
            customer.PhoneNumber = dto.PhoneNumber;

        if (!string.IsNullOrEmpty(dto.Address))
            customer.Address = dto.Address;

        if (!string.IsNullOrEmpty(dto.TaxNumber))
            customer.TaxNumber = dto.TaxNumber;

        if (dto.TotalBalance.HasValue)
            customer.TotalBalance = dto.TotalBalance.Value;

        if (dto.IsActive.HasValue)
            customer.IsActive = dto.IsActive.Value;

        var result = await _customerWriteRepository.UpdateAsync(customer, cancellationToken);

        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error occurred while updating the customer.", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Customer updated successfully.");

    }

    private GetCustomerDto MapToGetCustomerDto(Customer customer)
    {
        return new GetCustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            TaxNumber = customer.TaxNumber,
            CreatedAt = customer.CreatedAt,
            IsActive = customer.IsActive,
            TotalBalance = customer.TotalBalance
        };
    }
}
