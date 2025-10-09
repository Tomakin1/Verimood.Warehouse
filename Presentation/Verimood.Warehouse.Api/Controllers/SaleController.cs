using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Auth.Settings;
using Verimood.Warehouse.Application.Services.Sale.Interfaces;
using Verimood.Warehouse.Application.Services.Sale.Models;

namespace Verimood.Warehouse.Api.Controllers
{
    [Route("api/sales")]
    [ApiExplorerSettings(GroupName = "sale")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly ISaleService _saleService;

        public SaleController(ISaleService saleService)
        {
            _saleService = saleService;
        }

        /// <summary>
        /// Yeni satış oluşturur
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateSaleDto dto, CancellationToken cancellationToken)
        {
            var response = await _saleService.CreateAsync(dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Satışı günceller
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("update/{Id}")]
        public async Task<IActionResult> UpdateAsync(Guid Id, [FromBody] UpdateSaleDto dto, CancellationToken cancellationToken)
        {
            var response = await _saleService.UpdateAsync(Id, dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Satışı siler
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _saleService.DeleteAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Satışı Id ile getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _saleService.GetByIdAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Tüm satışları sayfalı şekilde getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("paginated")]
        public async Task<IActionResult> GetAllPaginatedAsync([FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            var response = await _saleService.GetAllPaginatedAsync(request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Belirli müşteriye ait satışları getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("customers/{CustomerId}")]
        public async Task<IActionResult> GetByCustomerIdAsync(Guid CustomerId, [FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            var response = await _saleService.GetByCustomerId(CustomerId, request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
