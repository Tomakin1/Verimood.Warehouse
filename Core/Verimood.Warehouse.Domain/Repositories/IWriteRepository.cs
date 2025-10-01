namespace Verimood.Warehouse.Domain.Repositories;

public interface IWriteRepository<T> where T : class
{
    Task<bool> CreateAsync(T entity, CancellationToken cancellationToken);
    Task<bool> CreateBulkAsync(IList<T> entities, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken);
    Task<bool> DeleteBulkAsync(IList<T> entities, CancellationToken cancellationToken);
    Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken);
    Task<bool> UpdateBulkAsync(List<T> entities, CancellationToken cancellationToken);
}
