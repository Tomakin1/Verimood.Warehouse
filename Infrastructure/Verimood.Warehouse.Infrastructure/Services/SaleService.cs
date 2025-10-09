using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Customer.Models;
using Verimood.Warehouse.Application.Services.Sale.Interfaces;
using Verimood.Warehouse.Application.Services.Sale.Models;
using Verimood.Warehouse.Application.Services.User.Interfaces;
using Verimood.Warehouse.Application.Services.User.Models;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;
using Verimood.Warehouse.Domain.Uow;

namespace Verimood.Warehouse.Infrastructure.Services;

public class SaleService : ISaleService
{
    private readonly IReadRepository<Sale> _saleReadRepository;
    private readonly IWriteRepository<Sale> _saleWriteRepository;
    private readonly IWriteRepository<SaleItem> _saleItemWriteRepository;
    private readonly IReadRepository<Product> _productReadRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWriteRepository<Product> _productWriteRepository;

    public SaleService(IReadRepository<Sale> saleReadRepository,
        IWriteRepository<Sale> saleWriteRepository,
        IWriteRepository<SaleItem> saleItemWriteRepository,
        IReadRepository<Product> productReadRepository,
        ICurrentUserService currentUser,
        IUnitOfWork unitOfWork,
        IWriteRepository<Product> productWriteRepository)
    {
        _saleReadRepository = saleReadRepository;
        _saleWriteRepository = saleWriteRepository;
        _saleItemWriteRepository = saleItemWriteRepository;
        _productReadRepository = productReadRepository;
        _currentUser = currentUser;
        _unitOfWork = unitOfWork;
        _productWriteRepository = productWriteRepository;
    }

    public async Task<BaseResponse<Guid>> CreateAsync(CreateSaleDto dto, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var sale = new Sale
            {
                CustomerId = dto.CustomerId,
                UserId = _currentUser.UserId,
                TotalAmount = 0
            };

            var result = await _saleWriteRepository.CreateAsync(sale, cancellationToken);
            if (!result)
            {
                return BaseResponse<Guid>.ErrorResponse("An error occurred while creating sale", 500);
            }


            if (dto.SaleItems != null && dto.SaleItems.Any())
            {
                var saleItems = new List<SaleItem>();

                foreach (var item in dto.SaleItems)
                {
                    var product = await _productReadRepository.GetByIdAsync(item.ProductId, cancellationToken);
                    if (product == null)
                    {
                        return BaseResponse<Guid>.ErrorResponse($"Product with id {item.ProductId} not found", 404);
                    }

                    if (product.StockQuantity < item.Quantity)
                    {
                        return BaseResponse<Guid>.ErrorResponse($"Insufficient stock for product {product.Name}", 400);
                    }

                    var saleItem = new SaleItem
                    {
                        SaleId = sale.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = product.Price,

                    };

                    saleItems.Add(saleItem);
                }

                await _saleItemWriteRepository.CreateBulkAsync(saleItems, cancellationToken);

                sale.TotalAmount = saleItems.Sum(x => x.TotalPrice);
                var saleWriteResult = await _saleWriteRepository.UpdateAsync(sale, cancellationToken);
                if (!saleWriteResult)
                {
                    return BaseResponse<Guid>.ErrorResponse("An error occurred while updating sale total amount", 500);
                }

                foreach (var item in saleItems)
                {
                    var product = await _productReadRepository.GetByIdAsync(item.ProductId, cancellationToken);
                    if (product != null)
                    {
                        product.StockQuantity -= item.Quantity;
                        var productUpdateResult = await _productWriteRepository.UpdateAsync(product, cancellationToken);
                        if (!productUpdateResult)
                        {
                            return BaseResponse<Guid>.ErrorResponse($"An error occurred while updating stock for product {product.Name}", 500);
                        }
                    }
                }

            }

            await transaction.CommitAsync(cancellationToken);
            return BaseResponse<Guid>.SuccessResponse(sale.Id, 201, "Sale created successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return BaseResponse<Guid>.ErrorResponse($"An error occurred while creating sale: {ex.Message}", 500);
        }
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken)
    {
        var sale = await _saleReadRepository.GetByIdAsync(Id, cancellationToken);
        if (sale == null)
        {
            return BaseResponse<object>.ErrorResponse("Sale not found", 404);
        }

        var result = await _saleWriteRepository.DeleteAsync(sale, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error ocurred while deleting sale", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Sale deleted successfully");
    }

    public async Task<BaseResponse<PaginationResponse<GetSaleDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _saleReadRepository.GetAllPaginatedAsync(
            cancellationToken,
            null,
            null,
            request.Page,
            request.PageSize,
            x => x.User,
            x => x.Customer);

        var sales = response.entities;
        var totalCount = response.totalCount;

        if (sales is null || !sales.Any())
        {
            return BaseResponse<PaginationResponse<GetSaleDto>>.ErrorResponse("No sales found.", 404);
        }

        return BaseResponse<PaginationResponse<GetSaleDto>>.SuccessResponse(
            new PaginationResponse<GetSaleDto>
            {
                Items = sales.Select(MapToGetSaleDto).ToList(),
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page
            }, 200,
            $"{totalCount} sales found");
    }

    public async Task<BaseResponse<PaginationResponse<GetSaleDto>>> GetByCustomerId(Guid CustomerId, PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _saleReadRepository.GetAllPaginatedAsync(
            cancellationToken,
            [x => x.CustomerId == CustomerId],
            x => x.OrderByDescending(x => x.SaleDate),
            request.Page,
            request.PageSize,
            x => x.User,
            x => x.Customer);

        var sales = response.entities;
        var totalCount = response.totalCount;

        if (sales is null || !sales.Any())
        {
            return BaseResponse<PaginationResponse<GetSaleDto>>.ErrorResponse("No sales found.", 404);
        }

        return BaseResponse<PaginationResponse<GetSaleDto>>.SuccessResponse(
            new PaginationResponse<GetSaleDto>
            {
                Items = sales.Select(MapToGetSaleDto).ToList(),
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page
            }, 200,
            $"{totalCount} sales found");
    }

    public async Task<BaseResponse<GetSaleDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        var sale = await _saleReadRepository.GetByIdAsync(Id, cancellationToken, x => x.User, x => x.Customer);
        if (sale == null)
        {
            return BaseResponse<GetSaleDto>.ErrorResponse("Sale not found", 404);
        }

        return BaseResponse<GetSaleDto>.SuccessResponse(MapToGetSaleDto(sale), 200, "Sale found");
    }

    public async Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateSaleDto dto, CancellationToken cancellationToken)
    {
        var sale = await _saleReadRepository.GetByIdAsync(Id, cancellationToken);
        if (sale == null)
        {
            return BaseResponse<object>.ErrorResponse("Sale not found", 404);
        }

        if (dto.CustomerId.HasValue)
            sale.CustomerId = dto.CustomerId.Value;

        if (dto.UserId.HasValue)
            sale.UserId = dto.UserId.Value;

        if (dto.TotalAmount.HasValue)
            sale.TotalAmount = dto.TotalAmount.Value;

        var result = await _saleWriteRepository.UpdateAsync(sale, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error ocurred while updating sale", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Sale updated successfully");
    }

    private GetSaleDto MapToGetSaleDto(Sale sale)
    {
        return new GetSaleDto
        {
            Id = sale.Id,
            TotalAmount = sale.TotalAmount,
            SaleDate = sale.SaleDate,
            Customer = new GetCustomerDto
            {
                Id = sale.Customer.Id,
                Name = sale.Customer.Name,
                Address = sale.Customer.Address,
                Email = sale.Customer.Email,
                IsActive = sale.Customer.IsActive,
                CreatedAt = sale.Customer.CreatedAt,
                PhoneNumber = sale.Customer.PhoneNumber,
                TaxNumber = sale.Customer.TaxNumber,
                TotalBalance = sale.Customer.TotalBalance
            },
            User = new GetUserDto
            {
                Id = sale.User.Id,
                FirstName = sale.User.FirstName,
                LastName = sale.User.LastName,
                Email = sale.User.Email ?? null,
                PhoneNumber = sale.User.PhoneNumber,
            }
        };
    }
}
