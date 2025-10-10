using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Persistence.Settings;

namespace Verimood.Warehouse.Persistence.Context;

public class ApplicationDbContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, UserNRole, IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    private readonly IHostEnvironment _env;
    private readonly DatabaseSettings _dbSettings;
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> opt,
        IOptions<DatabaseSettings> dbSettings,
        IHostEnvironment env) : base(opt)
    {
        _env = env;
        _configuration = configuration;

    }

    public DbSet<Category> Categories { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<CustomerBalance> CustomerBalances { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<SaleItem> SaleItems { get; set; }
    public DbSet<Stock> Stocks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (_env.IsDevelopment())
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        optionsBuilder.UseDatabase(_dbSettings.ConnectionString);

        base.OnConfiguring(optionsBuilder);

    }
}
