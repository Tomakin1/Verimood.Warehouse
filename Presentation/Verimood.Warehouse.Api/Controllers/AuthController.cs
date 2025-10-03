using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Verimood.Warehouse.Application.Services.Auth.Interfaces;
using Verimood.Warehouse.Application.Services.Auth.Models;
using Verimood.Warehouse.Application.Services.User.Interfaces;
using Verimood.Warehouse.Application.Services.User.Models;

namespace Verimood.Warehouse.Api.Controllers
{
    [Route("api/auth")]
    [ApiExplorerSettings(GroupName = "auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserAccountService _userAccountService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserAccountService userAccountService, ITokenService tokenService)
        {
            _userAccountService = userAccountService;
            _tokenService = tokenService;
        }

        /// <summary>
        /// Kullanıcı için giriş işlemi gerçekleştirir. 
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken cancellationToken = default!)
        {
            var response = await _userAccountService.Login(dto, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// refresh token atar ve yeni token döner.
        /// </summary>
        [HttpPost("refresh-token")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request, CancellationToken cancellationToken = default!)
        {
            var response = await _tokenService.RefreshAccesToken(request, cancellationToken);
            return StatusCode(response.StatusCode, response);
        }
    }
}
