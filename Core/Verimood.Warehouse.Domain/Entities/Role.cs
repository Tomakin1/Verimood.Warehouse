using Microsoft.AspNetCore.Identity;

namespace Verimood.Warehouse.Domain.Entities;

public class Role : IdentityRole<Guid>
{
    public string? Description { get; set; }
    public ICollection<UserNRole> UserRoles { get; set; } = new List<UserNRole>();

}
