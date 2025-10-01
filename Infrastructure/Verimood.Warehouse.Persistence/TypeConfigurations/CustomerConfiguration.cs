using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Verimood.Warehouse.Domain.Entities;

namespace Verimood.Warehouse.Persistence.TypeConfigurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Property(x => x.TaxNumber).HasMaxLength(50);
            builder.Property(x => x.Email).HasMaxLength(200);
            builder.Property(x => x.PhoneNumber).HasMaxLength(50);
            builder.Property(x => x.Address).HasMaxLength(1000);
            builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

            builder.HasMany(x => x.Sales).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.CustomerBalances).WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId).OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => x.IsActive).HasDatabaseName("ix_customers_isactive");
            builder.HasIndex(x => x.Id).HasDatabaseName("ix_customers_id");
        }
    }
}
