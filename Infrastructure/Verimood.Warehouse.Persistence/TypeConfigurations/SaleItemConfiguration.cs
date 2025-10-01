using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Verimood.Warehouse.Domain.Entities;

namespace Verimood.Warehouse.Persistence.TypeConfigurations;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.SaleId).IsRequired();
        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.Quantity).IsRequired();
        builder.Property(x => x.UnitPrice).IsRequired().HasColumnType("decimal(18,2)");


        builder.HasIndex(x => x.SaleId).HasDatabaseName("ix_saleitems_saleid");
        builder.HasIndex(x => x.ProductId).HasDatabaseName("ix_saleitems_productid");
    }
}
