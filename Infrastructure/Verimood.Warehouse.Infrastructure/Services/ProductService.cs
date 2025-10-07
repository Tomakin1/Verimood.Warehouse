using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Category.Models;
using Verimood.Warehouse.Application.Services.Product.Interfaces;
using Verimood.Warehouse.Application.Services.Product.Models;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;

namespace Verimood.Warehouse.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly IReadRepository<Product> _productReadRepository;
    private readonly IWriteRepository<Product> _productWriteRepository;

    public ProductService(IReadRepository<Product> productReadRepository, IWriteRepository<Product> productWriteRepository)
    {
        _productReadRepository = productReadRepository;
        _productWriteRepository = productWriteRepository;
    }

    public async Task<BaseResponse<Guid>> CreateAsync(CreateProductDto dto, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            CategoryId = dto.CategoryId,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQuantity = dto.StockQuantity,
            Barcode = dto.Barcode,
        };

        var result = await _productWriteRepository.CreateAsync(product, cancellationToken);
        if (!result)
        {
            return BaseResponse<Guid>.ErrorResponse("an error occured while creating product", 500);
        }

        return BaseResponse<Guid>.SuccessResponse(product.Id, 201, "Product created successfully");
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken)
    {
        var product = await _productReadRepository.GetByIdAsync(Id, cancellationToken);
        if (product is null)
        {
            return BaseResponse<object>.ErrorResponse("Product not found", 404);
        }

        var result = await _productWriteRepository.DeleteAsync(product, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error occurred while deleting the product.", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Product deleted successfully.");
    }

    public async Task<BaseResponse<PaginationResponse<GetProductDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _productReadRepository.GetAllPaginatedAsync(
            cancellationToken,
            null,
            x => x.OrderByDescending(x => x.CreatedAt),
            request.Page,
            request.PageSize,
            x => x.Category);

        var products = response.entities;
        var totalCount = response.totalCount;

        if (products is null || !products.Any())
        {
            return BaseResponse<PaginationResponse<GetProductDto>>.ErrorResponse("No products found.", 404);
        }

        return BaseResponse<PaginationResponse<GetProductDto>>.SuccessResponse(
            new PaginationResponse<GetProductDto>
            {
                Items = products.Select(MapToGetProductDto).ToList(),
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page
            }, 200,
            $"{totalCount} products found");
    }

    public async Task<BaseResponse<GetProductDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        var product = await _productReadRepository.GetByIdAsync(Id, cancellationToken, x => x.Category);
        if (product is null)
        {
            return BaseResponse<GetProductDto>.ErrorResponse("Product not found.", 404);
        }

        return BaseResponse<GetProductDto>.SuccessResponse(MapToGetProductDto(product), 200, "Product found.");
    }

    public async Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateProductDto dto, CancellationToken cancellationToken)
    {
        var product = await _productReadRepository.GetByIdAsync(Id, cancellationToken);
        if (product is null)
        {
            return BaseResponse<object>.ErrorResponse("Product not found", 404);
        }

        if (dto.CategoryId.HasValue)
            product.CategoryId = dto.CategoryId.Value;

        if (!string.IsNullOrEmpty(dto.Name))
            product.Name = dto.Name;

        if (!string.IsNullOrEmpty(dto.Description))
            product.Description = dto.Description;

        if (!string.IsNullOrEmpty(dto.Barcode))
            product.Barcode = dto.Barcode;

        if (dto.IsActive.HasValue)
            product.IsActive = dto.IsActive.Value;

        if (dto.Price.HasValue)
            product.Price = dto.Price.Value;

        if (dto.StockQuantity.HasValue)
            product.StockQuantity = dto.StockQuantity.Value;

        var result = await _productWriteRepository.UpdateAsync(product, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error occurred while updating the product.", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Product updated successfully.");
    }


    private GetProductDto MapToGetProductDto(Product product)
    {
        return new GetProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            Barcode = product.Barcode,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt,
            Category = new GetCategoryDto
            {
                Id = product.Category.Id,
                Name = product.Category.Name,
                Description = product.Category.Description,
                CreatedAt = product.Category.CreatedAt
            },

        };
    }
}
