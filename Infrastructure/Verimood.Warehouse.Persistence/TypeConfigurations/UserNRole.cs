using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Verimood.Warehouse.Domain.Entities;

namespace Verimood.Warehouse.Persistence.TypeConfigurations;

public class UserNRoleConfiguration : IEntityTypeConfiguration<UserNRole>
{
    public void Configure(EntityTypeBuilder<UserNRole> builder)
    {
        builder.ToTable("UserRoles");
        builder.HasKey(x => new { x.UserId, x.RoleId });

        builder.HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId).OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.UserId).HasDatabaseName("ix_usernroles_userid");
        builder.HasIndex(x => x.RoleId).HasDatabaseName("ix_usernroles_roleid");
    }
}
