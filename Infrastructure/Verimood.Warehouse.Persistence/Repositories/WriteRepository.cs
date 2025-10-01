using Verimood.Warehouse.Domain.Repositories;
using Verimood.Warehouse.Persistence.Context;

namespace Verimood.Warehouse.Persistence.Repositories;

public class WriteRepository<T> : IWriteRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;

    public WriteRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CreateAsync(T entity, CancellationToken cancellationToken)
    {
        await _context.Set<T>().AddAsync(entity, cancellationToken);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> CreateBulkAsync(IList<T> entity, CancellationToken cancellationToken)
    {
        await _context.Set<T>().AddRangeAsync(entity, cancellationToken);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteAsync(T entity, CancellationToken cancellationToken)
    {
        _context.Remove(entity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> DeleteBulkAsync(IList<T> entity, CancellationToken cancellationToken)
    {
        _context.RemoveRange(entity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken)
    {
        _context.Update(entity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }

    public async Task<bool> UpdateBulkAsync(List<T> entity, CancellationToken cancellationToken)
    {
        _context.Set<T>().UpdateRange(entity);
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}
