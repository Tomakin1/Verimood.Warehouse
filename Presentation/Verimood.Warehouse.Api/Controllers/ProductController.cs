using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Auth.Settings;
using Verimood.Warehouse.Application.Services.Product.Interfaces;
using Verimood.Warehouse.Application.Services.Product.Models;

namespace Verimood.Warehouse.Api.Controllers
{
    [Route("api/products")]
    [ApiExplorerSettings(GroupName = "product")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Yeni ürün oluşturur
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateProductDto dto, CancellationToken cancellationToken)
        {
            var response = await _productService.CreateAsync(dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Ürünü günceller
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("update/{Id}")]
        public async Task<IActionResult> UpdateAsync(Guid Id, [FromBody] UpdateProductDto dto, CancellationToken cancellationToken)
        {
            var response = await _productService.UpdateAsync(Id, dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Ürünü siler
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _productService.DeleteAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Ürünü Id ile getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _productService.GetByIdAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Tüm ürünleri sayfalı şekilde getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("paginated")]
        public async Task<IActionResult> GetAllPaginatedAsync([FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            var response = await _productService.GetAllPaginatedAsync(request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
