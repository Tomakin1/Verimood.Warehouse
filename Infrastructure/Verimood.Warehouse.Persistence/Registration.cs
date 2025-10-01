using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;
using Verimood.Warehouse.Domain.Uow;
using Verimood.Warehouse.Persistence.Context;
using Verimood.Warehouse.Persistence.Initialization;
using Verimood.Warehouse.Persistence.Repositories;
using Verimood.Warehouse.Persistence.Settings;
using Verimood.Warehouse.Persistence.Uow;

namespace Verimood.Warehouse.Persistence;

public static class Registration
{
    private const string Cors = nameof(Cors);

    public static IServiceCollection AddPersistence(this IServiceCollection services) // Gerekli methodların eklenmesi
    {
        AddCorsPolicy(services);
        AddDatabaseSettings(services);
        AddDatabaseContext(services);
        AddIdentity(services);
        AddRepositories(services);
        AddDatabaseInitializerProccess(services);
        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services) // Repository'leri DI Container'a Eklenmesi
    {
        services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    private static IServiceCollection AddDatabaseSettings(this IServiceCollection services)
    {
        services
        .AddOptions<DatabaseSettings>()
        .BindConfiguration(nameof(DatabaseSettings))
        .ValidateDataAnnotations()
        .ValidateOnStart();
        services.AddTransient(sp => sp.GetRequiredService<IOptions<DatabaseSettings>>().Value);

        return services;
    } // Konfigurasyon dosyasından bilgilerin bind edildiği modelin DI Container'a eklenmesi

    private static IServiceCollection AddDatabaseContext(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>((sp, context) =>
        {
            var dbSettings = sp.GetRequiredService<DatabaseSettings>();
            context.UseDatabase(dbSettings.ConnectionString);
        });

        return services;
    } // ApplicationDbContext ConnectionString Ayarı yapılması ve DI Container'a Eklenmesi

    private static IServiceCollection AddIdentity(IServiceCollection services)
    {
        services.AddIdentity<User, Role>(options =>
        {
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 1;

            options.User.RequireUniqueEmail = true;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            options.SignIn.RequireConfirmedEmail = true;
            options.SignIn.RequireConfirmedPhoneNumber = false;

        })
        .AddRoles<Role>()
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddSignInManager<SignInManager<User>>()
        .AddDefaultTokenProviders();
        return services;
    }  // Identity kütüphanesinden gelen default role user konfigurasyonları ve DI Container'a Eklenmesi

    internal static DbContextOptionsBuilder UseDatabase(this DbContextOptionsBuilder builder, string connectionString)
    {
        return builder.UseSqlServer(connectionString);

    } // Database bağlantısının sağlandığı nokta

    private static IServiceCollection AddCorsPolicy(this IServiceCollection services)
    {
        return services
                .AddCors(opt =>
                {
                    opt.AddPolicy(Cors,
                                    policy => policy.AllowAnyHeader()
                                                    .AllowAnyMethod()
                                                    .AllowAnyOrigin());
                });
    } // Cors policy konfigürasyonu ve DI Container'a eklenmesi

    private static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder builder)
    {
        return builder.UseCors(Cors);
    } // Cors Middleware

    public static IApplicationBuilder UsePersistenceLayer(this IApplicationBuilder app)
    {
        app.UseCorsPolicy()
           .UseAuthentication();

        app.ApplicationServices.StartDbInitializeAsync().Wait();

        return app;
    } // Persistence katmanında kullanılan Middleware'lar

    private static IServiceCollection AddDatabaseInitializerProccess(this IServiceCollection services)
    {
        services.AddTransient<IDatabaseInitializer, DatabaseInitializer>()
            .AddTransient<ApplicationDbSeeder>()
            .AddTransient<ApplicationDbInitializer>();
        return services;
    } // Database Initialize işlemlerini gerçekleştiren sınıflar DI Container'a Eklenmesi

    public static async Task StartDbInitializeAsync(this IServiceProvider sp, CancellationToken cancellationToken = default)
    {
        using var scope = sp.CreateScope();

        await scope.ServiceProvider
            .GetRequiredService<IDatabaseInitializer>()
            .InitializeDatabasesAsync(cancellationToken);
    } // Uygulamayla Beraber Db Initialize başladığı middleware
}
