using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace Verimood.Warehouse.Infrastructure.Settings.JWT;

public static class Startup
{
    internal static IServiceCollection AddJwtAuth(this IServiceCollection services)
    {
        services
        .AddOptions<JwtSettings>()
        .BindConfiguration(nameof(JwtSettings))
        .ValidateDataAnnotations()
        .ValidateOnStart();

        services.AddTransient(sp =>
        {
            return sp.GetRequiredService<IOptions<JwtSettings>>().Value;
        });

        services
            .AddSingleton<IConfigureOptions<JwtBearerOptions>, ConfigureJwtBearerOptions>();

        services
            .AddAuthentication(authentication =>
            {
                authentication.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                authentication.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
            });

        return services;
    }
}
