using Microsoft.Extensions.DependencyInjection;
using Verimood.Warehouse.Application.Services.Auth.Interfaces;
using Verimood.Warehouse.Application.Services.Category.Interfaces;
using Verimood.Warehouse.Application.Services.Customer.Interfaces;
using Verimood.Warehouse.Application.Services.CustomerBalance.Interfaces;
using Verimood.Warehouse.Application.Services.Product.Interfaces;
using Verimood.Warehouse.Application.Services.Role.Interfaces;
using Verimood.Warehouse.Application.Services.Sale.Interfaces;
using Verimood.Warehouse.Application.Services.SaleItem.Interfaces;
using Verimood.Warehouse.Application.Services.Stock.Interfaces;
using Verimood.Warehouse.Application.Services.User.Interfaces;
using Verimood.Warehouse.Infrastructure.Services;
using Verimood.Warehouse.Infrastructure.Settings.JWT;

namespace Verimood.Warehouse.Infrastructure;

public static class Registration
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddServices()
            .AddJwtAuth()
            .AddHttpContextAccessor();
        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IUserAccountService, UserAccountService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<ICustomerBalanceService, CustomerBalanceService>();
        services.AddScoped<ISaleService, SaleService>();
        services.AddScoped<ISaleItemService, SaleItemService>();
        services.AddScoped<IRoleService, RoleService>();


        return services;
    }
}
