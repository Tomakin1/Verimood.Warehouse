using Microsoft.AspNetCore.Builder;

namespace Verimood.Warehouse.Application.Exceptions;

public static class Startup
{
    public static void ConfigureExceptionHandling(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();

    }
}
