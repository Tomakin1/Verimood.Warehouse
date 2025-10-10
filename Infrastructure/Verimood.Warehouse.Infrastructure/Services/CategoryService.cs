using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Category.Interfaces;
using Verimood.Warehouse.Application.Services.Category.Models;
using Verimood.Warehouse.Application.Services.Product.Models;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;

namespace Verimood.Warehouse.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IReadRepository<Category> _categoryReadRepository;
    private readonly IReadRepository<Product> _productReadRepository;
    private readonly IWriteRepository<Category> _categoryWriteRepository;

    public CategoryService(IReadRepository<Category> categoryReadRepository, IReadRepository<Product> productReadRepository, IWriteRepository<Category> categoryWriteRepository)
    {
        _categoryReadRepository = categoryReadRepository;
        _productReadRepository = productReadRepository;
        _categoryWriteRepository = categoryWriteRepository;
    }

    public async Task<BaseResponse<Guid>> CreateAsync(CreateCategoryDto dto, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Name = dto.Name,
            Description = dto.Description
        };

        var result = await _categoryWriteRepository.CreateAsync(category, cancellationToken);
        if (!result)
        {
            return BaseResponse<Guid>.ErrorResponse("An error occurred while creating the category.", 500);
        }

        return BaseResponse<Guid>.SuccessResponse(category.Id, 201, "Category created successfully.");
    }

    public async Task<BaseResponse<object>> DeleteAsync(Guid Id, CancellationToken cancellationToken)
    {
        var category = await _categoryReadRepository.GetByIdAsync(Id, cancellationToken);
        if (category is null)
        {
            return BaseResponse<object>.ErrorResponse("Category not found.", 404);
        }

        var result = await _categoryWriteRepository.DeleteAsync(category, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error occurred while deleting the category.", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Category deleted successfully.");
    }

    public async Task<BaseResponse<PaginationResponse<GetCategoryDto>>> GetAllPaginatedAsync(PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _categoryReadRepository.GetAllPaginatedAsync(
            cancellationToken,
            null,
            x => x.OrderByDescending(x => x.CreatedAt),
            request.Page,
            request.PageSize,
            null);

        var categories = response.entities;
        var totalCount = response.totalCount;

        if (categories is null || !categories.Any())
        {
            return BaseResponse<PaginationResponse<GetCategoryDto>>.ErrorResponse("No categories found.", 404);
        }

        return BaseResponse<PaginationResponse<GetCategoryDto>>.SuccessResponse(
            new PaginationResponse<GetCategoryDto>
            {
                Items = categories.Select(MapToGetCategoryDto).ToList(),
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page
            }, 200,
            $"{totalCount} category found");
    }

    public async Task<BaseResponse<GetCategoryDto>> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
    {
        var category = await _categoryReadRepository.GetByIdAsync(Id, cancellationToken);
        if (category is null)
        {
            return BaseResponse<GetCategoryDto>.ErrorResponse("Category not found.", 404);
        }
        return BaseResponse<GetCategoryDto>.SuccessResponse(MapToGetCategoryDto(category), 200, "Category found.");
    }

    public async Task<BaseResponse<PaginationResponse<GetProductDto>>> GetCategoryProductsAsync(Guid Id, PaginationRequest request, CancellationToken cancellationToken)
    {
        var response = await _productReadRepository.GetAllPaginatedAsync(
            cancellationToken,
            [x => x.CategoryId == Id],
            x => x.OrderByDescending(x => x.CreatedAt),
            request.Page,
            request.PageSize,
            null);

        var products = response.entities;
        var totalCount = response.totalCount;   


        if (products is null || !products.Any())
        {
            return BaseResponse<PaginationResponse<GetProductDto>>.ErrorResponse("No categories found.", 404);
        }

        return BaseResponse<PaginationResponse<GetProductDto>>.SuccessResponse(
            new PaginationResponse<GetProductDto>
            {
                Items = products.Select(x => new GetProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = x.Price,
                    CreatedAt = x.CreatedAt,
                    Barcode = x.Barcode,
                    IsActive = x.IsActive,
                    StockQuantity = x.StockQuantity,
                    Category =  new GetCategoryDto
                    {
                        Id = x.Category.Id,
                        Name = x.Category.Name,
                        Description = x.Category.Description,
                        CreatedAt = x.Category.CreatedAt
                    }
                }).ToList(),
                TotalCount = totalCount,
                PageSize = request.PageSize,
                Page = request.Page
            }, 200,
            $"{totalCount} product found");
    }

    public async Task<BaseResponse<object>> UpdateAsync(Guid Id, UpdateCategoryDto dto, CancellationToken cancellationToken)
    {
        var category = await _categoryReadRepository.GetByIdAsync(Id, cancellationToken);
        if (category is null)
        {
            return BaseResponse<object>.ErrorResponse("Category not found.", 404);
        }

        if(!string.IsNullOrEmpty(dto.Name))
            category.Name = dto.Name;

        if(!string.IsNullOrEmpty(dto.Description))
            category.Description = dto.Description;

        if(dto.IsActive.HasValue)
            category.IsActive = dto.IsActive.Value;

        var result = await _categoryWriteRepository.UpdateAsync(category, cancellationToken);
        if (!result)
        {
            return BaseResponse<object>.ErrorResponse("An error occurred while updating the category.", 500);
        }

        return BaseResponse<object>.SuccessResponse(null, 204, "Category updated successfully.");
    }


    private GetCategoryDto MapToGetCategoryDto(Category category)
    {
        return new GetCategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            Description = category.Description,
            CreatedAt = category.CreatedAt,
            IsActive = category.IsActive
        };
    }
}
