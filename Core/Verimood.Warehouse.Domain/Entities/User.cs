using Microsoft.AspNetCore.Identity;

namespace Verimood.Warehouse.Domain.Entities;

public class User : IdentityUser<Guid>
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string FullName => $"{FirstName} {LastName}";

    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public ICollection<UserNRole> UserRoles { get; set; } = new List<UserNRole>();
    public ICollection<Sale>? Sales { get; set; }
}
