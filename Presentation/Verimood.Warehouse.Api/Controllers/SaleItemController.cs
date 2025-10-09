using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Auth.Settings;
using Verimood.Warehouse.Application.Services.SaleItem.Interfaces;
using Verimood.Warehouse.Application.Services.SaleItem.Models;

namespace Verimood.Warehouse.API.Controllers
{
    [Route("api/sale-items")]
    [ApiExplorerSettings(GroupName = "saleItem")]
    [ApiController]
    public class SaleItemController : ControllerBase
    {
        private readonly ISaleItemService _saleItemService;

        public SaleItemController(ISaleItemService saleItemService)
        {
            _saleItemService = saleItemService;
        }

        /// <summary>
        /// Yeni satış kalemleri (sale items) oluşturur (toplu ekleme)
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("create-bulk")]
        public async Task<IActionResult> CreateBulkAsync([FromBody] List<CreateSaleItemDto> dtos, CancellationToken cancellationToken)
        {
            var response = await _saleItemService.CreateBulkAsync(dtos, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Belirli bir satış kalemini günceller
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateSaleItemDto dto, CancellationToken cancellationToken)
        {
            var response = await _saleItemService.UpdateAsync(id, dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Belirli satış kalemlerini toplu olarak siler
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpDelete("delete-bulk")]
        public async Task<IActionResult> DeleteAsync([FromBody] List<Guid> ids, CancellationToken cancellationToken)
        {
            var response = await _saleItemService.DeleteAsync(ids, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Belirli bir satış kalemini ID’ye göre getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _saleItemService.GetByIdAsync(id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Tüm satış kalemlerini sayfalandırılmış şekilde getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("paginated")]
        public async Task<IActionResult> GetAllPaginatedAsync([FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            var response = await _saleItemService.GetAllPaginatedAsync(request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Belirli bir satışa (Sale) ait satış kalemlerini sayfalandırılmış şekilde getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("get-by-sale/{saleId:guid}")]
        public async Task<IActionResult> GetBySaleIdAsync(Guid saleId, [FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            var response = await _saleItemService.GetBySaleId(saleId, request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
