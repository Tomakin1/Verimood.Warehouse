using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Category.Models;
using Verimood.Warehouse.Application.Services.Product.Models;
using Verimood.Warehouse.Application.Services.Stock.Interfaces;
using Verimood.Warehouse.Application.Services.Stock.Models;
using Verimood.Warehouse.Application.Services.User.Interfaces;
using Verimood.Warehouse.Application.Services.User.Models;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;
using Verimood.Warehouse.Domain.Uow;

namespace Verimood.Warehouse.Infrastructure.Services;

public class StockService : IStockService
{
    private readonly IReadRepository<Stock> _stockReadRepository;
    private readonly IWriteRepository<Stock> _stockWriteRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IReadRepository<Product> _productReadRepository;
    private readonly IWriteRepository<Product> _productWriteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public StockService(
        IReadRepository<Stock> stockReadRepository,
        IWriteRepository<Stock> stockWriteRepository,
        ICurrentUserService currentUser,
        IReadRepository<Product> productReadRepository,
        IUnitOfWork unitOfWork,
        IWriteRepository<Product> productWriteRepository)
    {
        _stockReadRepository = stockReadRepository;
        _stockWriteRepository = stockWriteRepository;
        _currentUser = currentUser;
        _productReadRepository = productReadRepository;
        _unitOfWork = unitOfWork;
        _productWriteRepository = productWriteRepository;
    }

    public async Task<BaseResponse<Guid>> CreateAsync(CreateStockDto dto, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var creatorId = _currentUser.UserId;
            var stock = new Stock
            {
                UserId = creatorId,
                ProductId = dto.ProductId,
                Quantity = dto.Quantity,
                Description = dto.Description
            };

            var product = await _productReadRepository.GetByIdAsync(dto.ProductId, cancellationToken);

            if (product is null)
            {
                return BaseResponse<Guid>.ErrorResponse("Product not found.", 404);
            }

            product.StockQuantity += dto.Quantity;

            var productResult = await _productWriteRepository.UpdateAsync(product, cancellationToken);
            if(!productResult)
            {
                return BaseResponse<Guid>.ErrorResponse("An error occurred while updating the product stock quantity.", 500);
            }

            var stockResult = await _stockWriteRepository.CreateAsync(stock, cancellationToken);
            if (!stockResult)
            {
                return BaseResponse<Guid>.ErrorResponse("An error occurred while creating the stock entry.", 500);
            }

            await transaction.CommitAsync(cancellationToken);
            return BaseResponse<Guid>.SuccessResponse(stock.Id, 201, "Stock entry created successfully.");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return BaseResponse<Guid>.ErrorResponse(ex.Message, 500);
        }

    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken)
    {
        var stock = await _stockReadRepository.GetByIdAsync(Id, cancellationToken);
        if (stock is null)
        {
            return BaseResponse<object>.ErrorResponse("Stock entry not found.", 404);
        }

        var result = await _stockWriteRepository.DeleteAsync(stock, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error occurred while deleting the stock entry.", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Stock entry deleted successfully.");
    }

    public async Task<BaseResponse<PaginationResponse<GetStockDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _stockReadRepository.GetAllPaginatedAsync(
        cancellationToken,
        null,
        null,
        request.Page,
        request.PageSize,
        x => x.User!,
        x => x.Product,
        x => x.Product.Category);

        var stocks = response.entities;
        var totalCount = response.totalCount;

        if (stocks is null || !stocks.Any())
        {
            return BaseResponse<PaginationResponse<GetStockDto>>.ErrorResponse("No stock found.", 404);
        }

        return BaseResponse<PaginationResponse<GetStockDto>>.SuccessResponse(
            new PaginationResponse<GetStockDto>
            {
                Items = stocks.Select(MapGetStockDto).ToList(),
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page
            }, 200,
            $"{totalCount} stock found");
    }

    public async Task<BaseResponse<GetStockDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        var stock = await _stockReadRepository.GetByIdAsync(Id, cancellationToken, x => x.User!, x => x.Product, x => x.Product.Category);
        if (stock is null)
        {
            return BaseResponse<GetStockDto>.ErrorResponse("Stock entry not found.", 404);
        }

        return BaseResponse<GetStockDto>.SuccessResponse(MapGetStockDto(stock), 200, "Stock entry found.");
    }

    public async Task<BaseResponse<PaginationResponse<GetStockDto>>> GetByProductId(Guid productId, PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _stockReadRepository.GetAllPaginatedAsync(
        cancellationToken,
        [x => x.ProductId == productId],
        null,
        request.Page,
        request.PageSize,
        x => x.User!,
        x => x.Product,
        x => x.Product.Category);

        var stocks = response.entities;
        var totalCount = response.totalCount;

        if (stocks is null || !stocks.Any())
        {
            return BaseResponse<PaginationResponse<GetStockDto>>.ErrorResponse("No stock found.", 404);
        }

        return BaseResponse<PaginationResponse<GetStockDto>>.SuccessResponse(
            new PaginationResponse<GetStockDto>
            {
                Items = stocks.Select(MapGetStockDto).ToList(),
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page
            }, 200,
            $"{totalCount} stock found");
    }

    public async Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateStockDto dto, CancellationToken cancellationToken)
    {
        var stock = await _stockReadRepository.GetByIdAsync(Id, cancellationToken);
        if (stock is null)
        {
            return BaseResponse<object>.ErrorResponse("Stock entry not found.", 404);
        }

        if (!string.IsNullOrWhiteSpace(dto.Description))
            stock.Description = dto.Description;

        if (dto.Quantity.HasValue)
            stock.Quantity = dto.Quantity.Value;

        var result = await _stockWriteRepository.UpdateAsync(stock, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error occurred while updating the stock entry.", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Stock entry updated successfully.");
    }

    private GetStockDto MapGetStockDto(Stock stock)
    {
        return new GetStockDto
        {
            Id = stock.Id,
            Description = stock.Description,
            Quantity = stock.Quantity,
            TransactionDate = stock.TransactionDate,
            Product = new GetProductDto
            {
                Id = stock.Product.Id,
                Name = stock.Product.Name,
                Description = stock.Product.Description,
                Barcode = stock.Product.Barcode,
                CreatedAt = stock.Product.CreatedAt,
                IsActive = stock.Product.IsActive,
                Price = stock.Product.Price,
                StockQuantity = stock.Product.StockQuantity,
                Category = new GetCategoryDto
                {
                    Id = stock.Product.Category.Id,
                    Name = stock.Product.Category.Name,
                    Description = stock.Product.Category.Description,
                    IsActive = stock.Product.Category.IsActive,
                    CreatedAt = stock.Product.Category.CreatedAt
                }
            },
            User = new GetUserDto
            {
                Id = stock.User!.Id,
                FirstName = stock.User.FirstName,
                LastName = stock.User.LastName,
                Email = stock.User.Email ?? null,
                PhoneNumber = stock.User.PhoneNumber
            }
        };
    }
}
