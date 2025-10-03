using Verimood.Warehouse.Application.Helpers;
using Verimood.Warehouse.Application.Services.Auth.Models;
using Verimood.Warehouse.Application.Services.User.Models;

namespace Verimood.Warehouse.Application.Services.User.Interfaces;

public interface IUserAccountService
{
    Task<BaseResponse<TokenResponseModel>> Login(LoginDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> ChangePassword(ChangePasswordDto dto, CancellationToken cancellationToken);
    Task<BaseResponse<object>> Revoke(CancellationToken cancellationToken);



}
