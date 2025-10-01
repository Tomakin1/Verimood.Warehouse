using Microsoft.Extensions.DependencyInjection;
using Verimood.Warehouse.Application.Exceptions;

namespace Verimood.Warehouse.Application;

public static class Registration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ExceptionMiddleware>();
        return services;
    }
}
