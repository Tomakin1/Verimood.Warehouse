using Microsoft.AspNetCore.Identity;

namespace Verimood.Warehouse.Domain.Entities;

public class UserNRole : IdentityUserRole<Guid>
{
    public User User { get; set; } = default!;
    public Role Role { get; set; } = default!;
}
