using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Verimood.Warehouse.Persistence.Context;

namespace Verimood.Warehouse.Persistence.Initialization;

internal class ApplicationDbInitializer
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ApplicationDbSeeder _dbSeeder;
    private readonly ILogger<ApplicationDbInitializer> _logger;

    public ApplicationDbInitializer(ApplicationDbContext dbContext, ApplicationDbSeeder seeder, ILogger<ApplicationDbInitializer> logger)
    {
        _dbContext = dbContext;
        _dbSeeder = seeder;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        if (_dbContext.Database.GetMigrations().Any())
        {
            if ((await _dbContext.Database.GetPendingMigrationsAsync(ct)).Any())
            {
                _logger.LogInformation("Applying Migrations.");
                await _dbContext.Database.MigrateAsync(ct);
            }

            if (await _dbContext.Database.CanConnectAsync(ct))
            {
                _logger.LogInformation("Connection to Database Succeeded.");
                await _dbSeeder.SeedDatabaseAsync(ct);
            }
        }
    }
}
