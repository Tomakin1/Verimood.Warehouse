using System.Linq.Expressions;

namespace Verimood.Warehouse.Domain.Repositories;

public interface IReadRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid Id,
    CancellationToken cancellationToken,
    params Expression<Func<T, object>>[]? includes);
    Task<T?> GetByIdAsync(int Id,                              // İnteger Id'si bulunan tablo olursa generic olarak getirmek için eklendi 
    CancellationToken cancellationToken,
    params Expression<Func<T, object>>[]? includes);

    Task<T?> GetByFilterAsync(
    Expression<Func<T, bool>>[] filters,
    CancellationToken cancellationToken,
    params Expression<Func<T, object>>[]? includes);

    Task<List<T>> GetAllByFilterAsync(
    Expression<Func<T, bool>>[] filters,
    CancellationToken cancellationToken,
    params Expression<Func<T, object>>[]? includes);

    Task<(IList<T> entities, int totalCount)> GetAllPaginatedAsync(
    CancellationToken cancellationToken,
    Expression<Func<T, bool>>[]? filters,
    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
    int pageNumber = 1,
    int pageSize = 10,
    params Expression<Func<T, object>>[]? includes);
}

