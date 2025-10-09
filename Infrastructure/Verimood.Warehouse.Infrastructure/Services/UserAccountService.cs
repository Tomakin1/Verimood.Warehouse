using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using Verimood.Warehouse.Application.Exceptions.Models;
using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Auth.Interfaces;
using Verimood.Warehouse.Application.Services.Auth.Models;
using Verimood.Warehouse.Application.Services.User.Interfaces;
using Verimood.Warehouse.Application.Services.User.Models;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Infrastructure.Settings.JWT;

namespace Verimood.Warehouse.Infrastructure.Services;

public class UserAccountService : IUserAccountService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;
    private readonly JwtSettings _jwtSettings;

    public UserAccountService(ICurrentUserService currentUserService, UserManager<User> userManager, ITokenService tokenService, IOptions<JwtSettings> jwtSettings)
    {
        _currentUserService = currentUserService;
        _userManager = userManager;
        _tokenService = tokenService;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<BaseResponse<object>> ChangePassword(ChangePasswordDto dto, CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var existUser = await _userManager.FindByIdAsync(userId.ToString());

        if (existUser == null)
            return BaseResponse<object>.ErrorResponse("Please login", 403);

        var isOldPasswordCorrect = await _userManager.CheckPasswordAsync(existUser, dto.OldPassword);
        if (!isOldPasswordCorrect)
            return BaseResponse<object>.ErrorResponse("Old password is incorrect.", 400);

        var result = await _userManager.ChangePasswordAsync(existUser, dto.OldPassword, dto.NewPassword);

        if (result.Succeeded)
        {
            return BaseResponse<object>.SuccessResponse(null, 204, "Password changed successfully. Please login again.");
        }
        else
        {
            return BaseResponse<object>.ErrorResponse(string.Join(", ", result.Errors.Select(e => e.Description)), 500);
        }
    }

    public async Task<BaseResponse<TokenResponseModel>> Login(LoginDto dto, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return BaseResponse<TokenResponseModel>.ErrorResponse("Please login", 403);

        bool isPasswordCorrect = await _userManager.CheckPasswordAsync(user, dto.Password);
        if (!isPasswordCorrect)
            return BaseResponse<TokenResponseModel>.ErrorResponse("Password is incorrect.", 400);

        IList<string> roles = await _userManager.GetRolesAsync(user);

        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();


        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationInDays > 0 ? _jwtSettings.RefreshTokenExpirationInDays : 7);
        var result = await _userManager.UpdateAsync(user);

        if (!result.Succeeded)
            throw new InternalServerException("Login failed. Please try again.");


        return BaseResponse<TokenResponseModel>.SuccessResponse(new TokenResponseModel
        {
            Token = new JwtSecurityTokenHandler().WriteToken(accessToken),
            RefreshToken = refreshToken!,
            Expiration = accessToken!.ValidTo
        }, 200, "Login successfull");
    }

    public async Task<BaseResponse<object>> Revoke(CancellationToken cancellationToken)
    {
        var userId = _currentUserService.UserId;
        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
            return BaseResponse<object>.ErrorResponse("Please login", 403);

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.Now;

        return BaseResponse<object>.SuccessResponse(null, 204, "All Sessions Shut Down");
    }
}
