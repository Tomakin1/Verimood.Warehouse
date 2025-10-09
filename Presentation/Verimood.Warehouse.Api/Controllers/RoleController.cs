using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verimood.Warehouse.Application.Services.Auth.Settings;
using Verimood.Warehouse.Application.Services.Role.Interfaces;
using Verimood.Warehouse.Application.Services.Role.Models;

namespace Verimood.Warehouse.API.Controllers
{
    [Route("api/roles")]
    [ApiExplorerSettings(GroupName = "role")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Yeni rol oluşturur
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateRoleDto dto, CancellationToken cancellationToken)
        {
            var response = await _roleService.CreateAsync(dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Rolü günceller
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("update/{id}")]
        public async Task<IActionResult> UpdateAsync(Guid id, [FromBody] UpdateRoleDto dto, CancellationToken cancellationToken)
        {
            var response = await _roleService.UpdateAsync(id, dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Rolü siler
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _roleService.DeleteAsync(id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Rolü Id ile getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            var response = await _roleService.GetByIdAsync(id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Tüm rolleri getirir (sayfalama olmadan)
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync(CancellationToken cancellationToken)
        {
            var response = await _roleService.GetAllAsync(cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
