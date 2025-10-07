using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Auth.Settings;
using Verimood.Warehouse.Application.Services.CustomerBalance.Interfaces;
using Verimood.Warehouse.Application.Services.CustomerBalance.Models;

namespace Verimood.Warehouse.Api.Controllers
{
    [Route("api/customer-balances")]
    [ApiExplorerSettings(GroupName = "customerBalance")]
    [ApiController]
    public class CustomerBalanceController : ControllerBase
    {
        private readonly ICustomerBalanceService _customerBalanceService;

        public CustomerBalanceController(ICustomerBalanceService customerBalanceService)
        {
            _customerBalanceService = customerBalanceService;
        }

        /// <summary>
        /// Yeni bir müşteri bakiyesi oluşturur.
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateCustomerBalanceDto dto, CancellationToken cancellationToken)
        {
            var response = await _customerBalanceService.CreateAsync(dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Müşteri bakiyesini ID'ye göre günceller.
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("update/{id:guid}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateCustomerBalanceDto dto, CancellationToken cancellationToken)
        {
            var response = await _customerBalanceService.UpdateAsync(id, dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Müşteri bakiyesini ID’ye göre siler.
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _customerBalanceService.DeleteAsync(id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Tüm müşteri bakiyelerini sayfalama ile getirir.
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("paginated")]
        public async Task<IActionResult> GetAllPaginatedAsync([FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            var response = await _customerBalanceService.GetAllPaginatedAsync(request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Belirli bir müşteri ID’sine ait bakiyeleri sayfalama ile getirir.
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("customers/{customerId:guid}")]
        public async Task<IActionResult> GetByCustomerId(Guid customerId, [FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            var response = await _customerBalanceService.GetByCustomerIdAsync(customerId, request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Müşteri bakiyesini ID’ye göre getirir.
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var response = await _customerBalanceService.GetByIdAsync(id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
