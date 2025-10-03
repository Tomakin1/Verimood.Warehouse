using Verimood.Warehouse.Domain.Entities;

namespace Verimood.Warehouse.Domain.Repositories;

public interface IUserRepository
{
    Task<(List<User> users, int totalCount)> SearchAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken,
        string? searchTerm,
        bool? isActive);
}
