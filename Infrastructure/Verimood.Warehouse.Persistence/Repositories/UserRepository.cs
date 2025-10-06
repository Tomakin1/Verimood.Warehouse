using Microsoft.EntityFrameworkCore;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;
using Verimood.Warehouse.Persistence.Context;

namespace Verimood.Warehouse.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(List<User> users, int totalCount)> SearchAsync(int page, int pageSize, CancellationToken cancellationToken, string? searchTerm, bool? isActive)
    {
        var query = _context.Users
            .AsNoTracking();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var lowerSearchTerm = searchTerm.ToLower();

            query = query.Where(x => (x.FirstName + " " + x.LastName).ToLower().Contains(lowerSearchTerm) ||
                                      x.FirstName.ToLower().Contains(lowerSearchTerm) ||
                                      x.LastName.ToLower().Contains(lowerSearchTerm) ||
                                      x.Email!.ToLower().Contains(lowerSearchTerm)
            );
        }

        if (isActive.HasValue)
        {
            query = query.Where(x => x.IsActive == isActive.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var users = await query
        .OrderByDescending(x => x.CreatedAt)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);

        return (users, totalCount);
    }
}
