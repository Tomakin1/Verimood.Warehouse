using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Product.Models;
using Verimood.Warehouse.Application.Services.SaleItem.Interfaces;
using Verimood.Warehouse.Application.Services.SaleItem.Models;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;
using Verimood.Warehouse.Domain.Uow;

namespace Verimood.Warehouse.Infrastructure.Services;

public class SaleItemService : ISaleItemService
{
    private readonly IReadRepository<SaleItem> _saleItemReadRepository;
    private readonly IWriteRepository<SaleItem> _saleItemWriteRepository;
    private readonly IReadRepository<Product> _productReadRepository;
    private readonly IUnitOfWork _unitOfWork;

    public SaleItemService(IReadRepository<SaleItem> saleItemReadRepository,
        IWriteRepository<SaleItem> saleItemWriteRepository,
        IReadRepository<Product> productReadRepository,
        IUnitOfWork unitOfWork)
    {
        _saleItemReadRepository = saleItemReadRepository;
        _saleItemWriteRepository = saleItemWriteRepository;
        _productReadRepository = productReadRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BaseResponse<List<Guid>>> CreateBulkAsync(List<CreateSaleItemDto> dtos, CancellationToken cancellationToken)
    {
        var transaction = await _unitOfWork.BeginTransactionAsync(cancellationToken);

        try
        {
            var saleItems = new List<SaleItem>();

            foreach (var dto in dtos)
            {
                var product = await _productReadRepository.GetByIdAsync(dto.ProductId, cancellationToken);
                if (product == null)
                {
                    return BaseResponse<List<Guid>>.ErrorResponse($"Product with id {dto.ProductId} not found", 404);
                }

                var saleItem = new SaleItem
                {
                    SaleId = dto.SaleId,
                    ProductId = dto.ProductId,
                    Quantity = dto.Quantity,
                    UnitPrice = product.Price,
                };
                saleItems.Add(saleItem);
            }

            var result = await _saleItemWriteRepository.CreateBulkAsync(saleItems, cancellationToken);
            if (!result)
            {
                return BaseResponse<List<Guid>>.ErrorResponse("An error ocurred while creating sale items", 500);
            }

            return BaseResponse<List<Guid>>.SuccessResponse(saleItems.Select(x => x.Id).ToList(), 201, "Sale items created successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            return BaseResponse<List<Guid>>.ErrorResponse($"An error ocurred while creating sale items : {ex.Message}", 500);
        }

    }
    public async Task<BaseResponse<object>> DeleteAsync(List<Guid> ids, CancellationToken cancellationToken)
    {
        if (ids is null || !ids.Any())
            return BaseResponse<object>.ErrorResponse("No sale item IDs provided", 400);

        var itemsToDelete = new List<SaleItem>();
        var notFoundIds = new List<Guid>();

        foreach (var id in ids)
        {
            var saleItem = await _saleItemReadRepository.GetByIdAsync(id, cancellationToken);
            if (saleItem is null)
            {
                notFoundIds.Add(id);
                continue;
            }

            itemsToDelete.Add(saleItem);
        }

        if (!itemsToDelete.Any())
            return BaseResponse<object>.ErrorResponse("No valid sale items found to delete", 404);

        var deleteResult = await _saleItemWriteRepository.DeleteBulkAsync(itemsToDelete, cancellationToken);

        if (!deleteResult)
            return BaseResponse<object>.ErrorResponse("An error occurred while deleting sale items", 500);

        var message = notFoundIds.Any()
            ? $"Sale items deleted successfully. Undeleted items (not found): {string.Join(", ", notFoundIds)}"
            : "All sale items deleted successfully.";

        return BaseResponse<object>.SuccessResponse(null, 204, message);
    }


    public async Task<BaseResponse<PaginationResponse<GetSaleItemDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _saleItemReadRepository.GetAllPaginatedAsync(
        cancellationToken,
        null,
        null,
        request.Page,
        request.PageSize,
        x => x.Product
        );

        var saleItems = response.entities;
        var totalCount = response.totalCount;

        if (saleItems is null || !saleItems.Any())
        {
            return BaseResponse<PaginationResponse<GetSaleItemDto>>.ErrorResponse("No saleItems found.", 404);
        }

        return BaseResponse<PaginationResponse<GetSaleItemDto>>.SuccessResponse(
            new PaginationResponse<GetSaleItemDto>
            {
                Items = saleItems.Select(MapToGetSaleItemDto).ToList(),
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page
            }, 200,
            $"{totalCount} : saleItems found");
    }

    public async Task<BaseResponse<GetSaleItemDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        var saleItem = await _saleItemReadRepository.GetByIdAsync(Id, cancellationToken, x => x.Product);
        if (saleItem is null)
        {
            return BaseResponse<GetSaleItemDto>.ErrorResponse("Sale not found", 404);
        }

        return BaseResponse<GetSaleItemDto>.SuccessResponse(MapToGetSaleItemDto(saleItem), 200, "Sale found");
    }

    public async Task<BaseResponse<PaginationResponse<GetSaleItemDto>>> GetBySaleId(Guid saleId, PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _saleItemReadRepository.GetAllPaginatedAsync(
        cancellationToken,
        [x => x.SaleId == saleId],
        null,
        request.Page,
        request.PageSize,
        x => x.Product
        );

        var saleItems = response.entities;
        var totalCount = response.totalCount;

        if (saleItems is null || !saleItems.Any())
        {
            return BaseResponse<PaginationResponse<GetSaleItemDto>>.ErrorResponse("No saleItems found.", 404);
        }

        return BaseResponse<PaginationResponse<GetSaleItemDto>>.SuccessResponse(
            new PaginationResponse<GetSaleItemDto>
            {
                Items = saleItems.Select(MapToGetSaleItemDto).ToList(),
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page
            }, 200,
            $"{totalCount} : saleItems found");
    }

    public async Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateSaleItemDto dto, CancellationToken cancellationToken)
    {
        var saleItem = await _saleItemReadRepository.GetByIdAsync(Id, cancellationToken);

        if (saleItem is null)
        {
            return BaseResponse<object>.ErrorResponse("Sale item not found", 404);
        }

        if (dto.ProductId.HasValue && dto.ProductId != saleItem.ProductId)
        {
            var product = await _productReadRepository.GetByIdAsync(dto.ProductId.Value, cancellationToken);
            if (product is null)
            {
                return BaseResponse<object>.ErrorResponse("Product not found", 404);
            }
            saleItem.ProductId = dto.ProductId.Value;
            saleItem.UnitPrice = product.Price;
        }

        if (dto.Quantity.HasValue)
        {
            saleItem.Quantity = dto.Quantity.Value;
        }

        var result = await _saleItemWriteRepository.UpdateAsync(saleItem, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error ocurred while updating sale item", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Sale item updated successfully");
    }

    private GetSaleItemDto MapToGetSaleItemDto(SaleItem saleItem)
    {
        return new GetSaleItemDto
        {
            Id = saleItem.Id,
            Quantity = saleItem.Quantity,
            UnitPrice = saleItem.UnitPrice,
            CreatedAt = saleItem.CreatedAt,
            Product = new GetProductDto
            {
                Id = saleItem.Product.Id,
                Name = saleItem.Product.Name,
                Description = saleItem.Product.Description,
                Price = saleItem.Product.Price,
                StockQuantity = saleItem.Product.StockQuantity,
                CreatedAt = saleItem.Product.CreatedAt,
                Barcode = saleItem.Product.Barcode,
                IsActive = saleItem.Product.IsActive
            }
        };
    }
}
