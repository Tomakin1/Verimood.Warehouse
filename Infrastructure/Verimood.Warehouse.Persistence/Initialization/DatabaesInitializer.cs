using Microsoft.Extensions.DependencyInjection;

namespace Verimood.Warehouse.Persistence.Initialization;

internal class DatabaseInitializer : IDatabaseInitializer
{
    private readonly IServiceProvider _serviceProvider;

    public DatabaseInitializer(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task InitializeDatabasesAsync(CancellationToken ct = default)
    {
        using var scope = _serviceProvider.CreateScope();
        await scope.ServiceProvider
            .GetRequiredService<ApplicationDbInitializer>()
            .InitializeAsync(ct);
    }
}
