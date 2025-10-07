using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Auth.Settings;
using Verimood.Warehouse.Application.Services.Category.Interfaces;
using Verimood.Warehouse.Application.Services.Category.Models;

namespace Verimood.Warehouse.Api.Controllers
{
    [Route("api/categories")]
    [ApiExplorerSettings(GroupName = "category")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Yeni kategori oluşturur
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCategoryDto dto, CancellationToken cancellationToken)
        {
            var response = await _categoryService.CreateAsync(dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Kategoriyi günceller
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("update/{Id}")]
        public async Task<IActionResult> UpdateAsync(Guid Id, [FromBody] UpdateCategoryDto dto, CancellationToken cancellationToken)
        {
            var response = await _categoryService.UpdateAsync(Id, dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Kategoriyi siler
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _categoryService.DeleteAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Kategoriyi Id ile getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _categoryService.GetByIdAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Tüm kategorileri sayfalı şekilde getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("paginated")]
        public async Task<IActionResult> GetAllPaginatedAsync([FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            var response = await _categoryService.GetAllPaginatedAsync(request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Belirli kategoriye ait ürünleri getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("{Id}/products")]
        public async Task<IActionResult> GetCategoryProductsAsync(Guid Id, [FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            var response = await _categoryService.GetCategoryProductsAsync(Id, request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
