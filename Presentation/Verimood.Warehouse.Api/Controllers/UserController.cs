using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Auth.Settings;
using Verimood.Warehouse.Application.Services.User.Interfaces;
using Verimood.Warehouse.Application.Services.User.Models;

namespace Verimood.Warehouse.Api.Controllers
{
    [Route("api/users")]
    [ApiExplorerSettings(GroupName = "user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IUserAccountService _userAccountService;

        public UserController(IUserService userService, IUserAccountService userAccountService)
        {
            _userService = userService;
            _userAccountService = userAccountService;
        }



        /// <summary>
        /// Admin hesabından yeni bir çalışana hesap açma
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateUserDto dto, CancellationToken cancellationToken)
        {
            var response = await _userService.CreateAsync(dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Admin hesabından çalışan bilgileri güncelleme
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("update/{Id}")]
        public async Task<IActionResult> UpdateAsync(Guid Id, [FromBody] UpdateUserDto dto, CancellationToken cancellationToken)
        {
            var response = await _userService.UpdateAsync(Id, dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Admin hesabından çalışanı kalıcı olarak sistemden silme
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpDelete("{Id}")]
        public async Task<IActionResult> DeleteAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _userService.DeleteAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Id'ye bağlı olarak kullanıcı bilgisi getirir
        /// </summary>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpGet("{Id}")]
        public async Task<IActionResult> GetByIdAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _userService.GetByIdAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Sistemdeki tüm kullanıcıları sayfalayarak getirir
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("paginated")]
        public async Task<IActionResult> GetAllPaginatedAsync([FromBody] PaginationRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.GetAllPaginatedAsync(request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Admin hesabından Pasifteki kullanıcıyı aktife çeker
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("activate/{Id}")]
        public async Task<IActionResult> ActivateAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _userService.ActivateAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Admin hesabından Aktif kullanıcıyı pasife çeker
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("deactive/{Id}")]
        public async Task<IActionResult> DeactiveAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _userService.DeactivateAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Id'si verilen kullanıcının rollerini getirir
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("roles/{Id}")]
        public async Task<IActionResult> GetRolesAsync(Guid Id, CancellationToken cancellationToken)
        {
            var response = await _userService.GetRolesAsync(Id, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Giriş yapan kullanıcının şifresini değiştirir.
        /// </summary>
        /// <param name="request">Yeni şifre bilgileri</param>
        /// <param name="cancellationToken">İptal belirteci</param>
        /// <returns>Başarı mesajı</returns>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request, CancellationToken cancellationToken = default!)
        {
            var response = await _userAccountService.ChangePassword(request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Kullanıcının oturumunu sonlandırır (token iptali).
        /// </summary>
        /// <returns>Başarı mesajı</returns>
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Employee}")]
        [HttpPost("logout")]
        public async Task<IActionResult> Revoke(CancellationToken cancellationToken = default!)
        {
            var response = await _userAccountService.Revoke(cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// verilen kriterlere göre kullanıcı getirir
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("search")]
        public async Task<IActionResult> SearchAsync([FromBody] SearchFilterUserDto dto,CancellationToken cancellationToken)
        {
            var response = await _userService.SearchAsync(dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Kullanıcıya rol atar
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRoleAsync([FromBody] AssignRoleDto dto, CancellationToken cancellationToken)
        {
            var response = await _userService.AssignRoleAsync(dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// Atanan rolü kullanıcıdan kaldırır
        /// </summary>
        [Authorize(Roles = AppRoles.Admin)]
        [HttpPost("remove-role")]
        public async Task<IActionResult> RemoveRoleAsync([FromBody] AssignRoleDto dto, CancellationToken cancellationToken)
        {
            var response = await _userService.RemoveRoleAsync(dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

    }
}
