using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Verimood.Warehouse.Domain.Entities;

namespace Verimood.Warehouse.Persistence.TypeConfigurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.CustomerId).IsRequired();
        builder.Property(x => x.UserId).IsRequired();
        builder.Property(x => x.TotalAmount).IsRequired().HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.User).WithMany(x => x.Sales).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
        builder.HasMany(x => x.SaleItems).WithOne(x => x.Sale).HasForeignKey(x => x.SaleId).OnDelete(DeleteBehavior.NoAction);

        builder.HasIndex(x => x.CustomerId).HasDatabaseName("ix_sales_customerid");
        builder.HasIndex(x => x.UserId).HasDatabaseName("ix_sales_userid");
        builder.HasIndex(x => x.SaleDate).HasDatabaseName("ix_sales_saledate");
    }
}
