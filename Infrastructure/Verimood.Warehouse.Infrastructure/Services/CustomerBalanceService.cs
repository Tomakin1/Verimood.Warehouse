using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Customer.Models;
using Verimood.Warehouse.Application.Services.CustomerBalance.Interfaces;
using Verimood.Warehouse.Application.Services.CustomerBalance.Models;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;
using Verimood.Warehouse.Domain.Uow;

namespace Verimood.Warehouse.Infrastructure.Services;

public class CustomerBalanceService : ICustomerBalanceService
{
    private readonly IReadRepository<CustomerBalance> _customerBalanceReadRepository;
    private readonly IReadRepository<Customer> _customerReadRepository;
    private readonly IWriteRepository<CustomerBalance> _customerBalanceWriteRepository;
    private readonly IWriteRepository<Customer> _customerWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CustomerBalanceService(
        IReadRepository<CustomerBalance> customerBalanceReadRepository,
        IReadRepository<Customer> customerReadRepository,
        IWriteRepository<CustomerBalance> customerBalanceWriteRepository,
        IWriteRepository<Customer> customerWriteRepository,
        IUnitOfWork unitOfWork)
    {
        _customerBalanceReadRepository = customerBalanceReadRepository;
        _customerReadRepository = customerReadRepository;
        _customerBalanceWriteRepository = customerBalanceWriteRepository;
        _customerWriteRepository = customerWriteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<Guid>> CreateAsync(CreateCustomerBalanceDto dto, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var customer = await _customerReadRepository.GetByIdAsync(dto.CustomerId, cancellationToken);

            if (customer is null)
            {
                return BaseResponse<Guid>.ErrorResponse("Customer not found", 404);
            }

            var customerBalance = new CustomerBalance
            {
                CustomerId = dto.CustomerId,
                Balance = dto.Balance,
                Description = dto.Description,
            };


            var result = await _customerBalanceWriteRepository.CreateAsync(customerBalance, cancellationToken);
            if (!result)
            {
                return BaseResponse<Guid>.ErrorResponse("An error occurred while creating customer balance", 500);
            }

            customer.TotalBalance += dto.Balance;


            var customerResult = await _customerWriteRepository.UpdateAsync(customer, cancellationToken);
            if (!customerResult)
            {
                return BaseResponse<Guid>.ErrorResponse("An error occurred while updating customer total balance", 500);
            }

            await transaction.CommitAsync(cancellationToken);
            return BaseResponse<Guid>.SuccessResponse(customerBalance.Id, 201, "Customer balance created successfully");
        }
        catch (
            Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return BaseResponse<Guid>.ErrorResponse("An error occurred while creating customer balance: " + ex.Message, 500);
        }


    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken)
    {
        var customerBalance = await _customerBalanceReadRepository.GetByIdAsync(Id, cancellationToken);
        if (customerBalance is null)
        {
            return BaseResponse<object>.ErrorResponse("Customer balance not found", 404);
        }

        var result = await _customerBalanceWriteRepository.DeleteAsync(customerBalance, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error occurred while deleting customer balance", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Customer balance deleted successfully");

    }

    public async Task<BaseResponse<PaginationResponse<GetCustomerBalanceDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _customerBalanceReadRepository.GetAllPaginatedAsync(
                   cancellationToken,
                   null,
                   x => x.OrderByDescending(x => x.CreatedAt),
                   request.Page,
                   request.PageSize,
                   x => x.Customer);

        var customerBalances = response.entities;
        var totalCount = response.totalCount;

        if (customerBalances is null || !customerBalances.Any())
        {
            return BaseResponse<PaginationResponse<GetCustomerBalanceDto>>.ErrorResponse("No customers found.", 404);
        }

        return BaseResponse<PaginationResponse<GetCustomerBalanceDto>>.SuccessResponse(
            new PaginationResponse<GetCustomerBalanceDto>
            {
                Items = customerBalances.Select(MapToGetCustomerBalanceDto).ToList(),
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page
            }, 200,
            $"{totalCount} customers found");
    }

    public async Task<BaseResponse<PaginationResponse<GetCustomerBalanceDto>>> GetByCustomerIdAsync(Guid customerId, PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _customerBalanceReadRepository.GetAllPaginatedAsync(
                   cancellationToken,
                   [x => x.CustomerId == customerId],
                   x => x.OrderByDescending(x => x.CreatedAt),
                   request.Page,
                   request.PageSize,
                   x => x.Customer);

        var customerBalances = response.entities;
        var totalCount = response.totalCount;

        if (customerBalances is null || !customerBalances.Any())
        {
            return BaseResponse<PaginationResponse<GetCustomerBalanceDto>>.ErrorResponse("No customers found.", 404);
        }

        return BaseResponse<PaginationResponse<GetCustomerBalanceDto>>.SuccessResponse(
            new PaginationResponse<GetCustomerBalanceDto>
            {
                Items = customerBalances.Select(MapToGetCustomerBalanceDto).ToList(),
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page
            }, 200,
            $"{totalCount} customers found");
    }

    public async Task<BaseResponse<GetCustomerBalanceDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        var customerBalance = await _customerBalanceReadRepository.GetByIdAsync(Id, cancellationToken, x => x.Customer);

        if (customerBalance is null)
        {
            return BaseResponse<GetCustomerBalanceDto>.ErrorResponse("Customer balance not found", 404);
        }

        return BaseResponse<GetCustomerBalanceDto>.SuccessResponse(MapToGetCustomerBalanceDto(customerBalance), 200, "Customer balance found");
    }

    public async Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateCustomerBalanceDto dto, CancellationToken cancellationToken)
    {
        var customerBalance = await _customerBalanceReadRepository.GetByIdAsync(Id, cancellationToken, x => x.Customer);

        if (customerBalance is null)
        {
            return BaseResponse<object>.ErrorResponse("Customer balance not found", 404);
        }

        if (dto.Balance.HasValue)
        {
            customerBalance.Customer.TotalBalance -= customerBalance.Balance;
            customerBalance.Balance = dto.Balance.Value;
            customerBalance.Customer.TotalBalance += dto.Balance.Value;

            var customerResult = await _customerWriteRepository.UpdateAsync(customerBalance.Customer, cancellationToken);
            if (!customerResult)
            {
                return BaseResponse<object>.ErrorResponse("An error occurred while updating customer total balance", 500);
            }

        }

        if (!string.IsNullOrWhiteSpace(dto.Description))
            customerBalance.Description = dto.Description;

        var result = await _customerBalanceWriteRepository.UpdateAsync(customerBalance, cancellationToken);

        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error occurred while updating customer balance", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Customer balance updated successfully");
    }

    private GetCustomerBalanceDto MapToGetCustomerBalanceDto(CustomerBalance customerBalance)
    {
        return new GetCustomerBalanceDto
        {
            Id = customerBalance.Id,
            Balance = customerBalance.Balance,
            Description = customerBalance.Description,
            CreatedAt = customerBalance.CreatedAt,
            Customer = new GetCustomerDto
            {
                Id = customerBalance.Customer.Id,
                Name = customerBalance.Customer.Name,
                Email = customerBalance.Customer.Email,
                PhoneNumber = customerBalance.Customer.PhoneNumber,
                Address = customerBalance.Customer.Address,
                TotalBalance = customerBalance.Customer.TotalBalance,
                CreatedAt = customerBalance.Customer.CreatedAt,
                TaxNumber = customerBalance.Customer.TaxNumber,
                IsActive = customerBalance.Customer.IsActive
            }
        };
    }
}
