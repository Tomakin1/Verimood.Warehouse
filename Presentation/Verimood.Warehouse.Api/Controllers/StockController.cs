using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Auth.Settings;
using Verimood.Warehouse.Application.Services.Stock.Interfaces;
using Verimood.Warehouse.Application.Services.Stock.Models;

namespace Verimood.Warehouse.Api.Controllers
{
    [Route("api/stocks")]
    [ApiExplorerSettings(GroupName = "stock")]
    [ApiController]
    public class StockController : ControllerBase
    {
        private readonly IStockService _stockService;

        public StockController(IStockService stockService)
        {
            _stockService = stockService;
        }

        /// <summary>
        /// Yeni stok girişi oluşturur
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateStockDto dto, CancellationToken cancellationToken)
        {
            var response = await _stockService.CreateAsync(dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Stok kaydını günceller
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("update/{Id}")]
        public async Task<IActionResult> UpdateAsync(Guid Id, [FromBody] UpdateStockDto dto, CancellationToken cancellationToken)
        {
            var response = await _stockService.UpdateAsync(Id, dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Stok kaydını siler
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _stockService.DeleteAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Id ile stok kaydını getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _stockService.GetByIdAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Tüm stokları sayfalı şekilde getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("paginated")]
        public async Task<IActionResult> GetAllPaginatedAsync([FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            var response = await _stockService.GetAllPaginatedAsync(request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Belirli bir ürünün stok kayıtlarını getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("products/{productId}")]
        public async Task<IActionResult> GetByProductIdAsync(Guid productId, [FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            var response = await _stockService.GetByProductId(productId, request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
