using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Verimood.Warehouse.Domain.Entities;

namespace Verimood.Warehouse.Persistence.TypeConfigurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.CategoryId).IsRequired();
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Description).HasMaxLength(2000);
        builder.Property(x => x.Barcode).HasMaxLength(100);
        builder.Property(x => x.Price).IsRequired().HasColumnType("decimal(18,2)");
        builder.Property(x => x.StockQuantity).IsRequired();
        builder.Property(x => x.IsActive).IsRequired().HasDefaultValue(true);

        builder.HasMany(x => x.Stocks).WithOne(x => x.Product).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x => x.SaleItems).WithOne(x => x.Product).HasForeignKey(x => x.ProductId).OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.CategoryId).HasDatabaseName("ix_products_categoryid");
        builder.HasIndex(x => x.IsActive).HasDatabaseName("ix_products_isactive");
        builder.HasIndex(x => x.Id).HasDatabaseName("ix_products_id");
        builder.HasIndex(x => x.Barcode).HasDatabaseName("ix_products_barcode");
    }
}
