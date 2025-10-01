using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Verimood.Warehouse.Domain.Entities;

namespace Verimood.Warehouse.Persistence.TypeConfigurations;

public class CustomerBalanceConfiguration : IEntityTypeConfiguration<CustomerBalance>
{
    public void Configure(EntityTypeBuilder<CustomerBalance> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.Debit).IsRequired().HasDefaultValue(0).HasDefaultValueSql("decimal(18,2)");
        builder.Property(x => x.Credit).IsRequired().HasDefaultValue(0).HasDefaultValueSql("decimal(18,2)");
        builder.Property(x => x.Description).HasMaxLength(1000);


        builder.HasIndex(x => x.CustomerId).HasDatabaseName("ix_customerbalances_customerid");
        builder.HasIndex(x => x.CreatedAt).HasDatabaseName("ix_customerbalances_createdat");
    }
}
