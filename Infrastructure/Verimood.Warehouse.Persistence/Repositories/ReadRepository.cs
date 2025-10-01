using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Verimood.Warehouse.Domain.Repositories;
using Verimood.Warehouse.Persistence.Context;

namespace Verimood.Warehouse.Persistence.Repositories;

public class ReadRepository<T> : IReadRepository<T> where T : class
{
    private readonly ApplicationDbContext _context;

    public ReadRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<T>> GetAllByFilterAsync(Expression<Func<T, bool>>[] filters, CancellationToken cancellationToken, params Expression<Func<T, object>>[]? includes)
    {
        var query = _context.Set<T>().AsQueryable();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        if (filters != null)
        {
            foreach (var filter in filters)
            {
                query = query.Where(filter);
            }
        }

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<(IList<T> entities, int totalCount)> GetAllPaginatedAsync(CancellationToken cancellationToken, Expression<Func<T, bool>>[]? filters, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int pageNumber = 1, int pageSize = 10, params Expression<Func<T, object>>[]? includes)
    {
        var query = _context.Set<T>().AsQueryable().AsNoTracking();

        if (filters is not null)
        {
            foreach (var filter in filters)
            {
                query = query.Where(filter);
            }
        }

        if (includes is not null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        var totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        var entities = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (entities, totalCount);
    }

    public async Task<T?> GetByFilterAsync(Expression<Func<T, bool>>[] filters, CancellationToken cancellationToken, params Expression<Func<T, object>>[]? includes)
    {
        var query = _context.Set<T>().AsQueryable();

        if (includes != null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        if (filters != null)
        {
            foreach (var filter in filters)
            {
                query = query.Where(filter);
            }
        }

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid Id, CancellationToken cancellationToken, params Expression<Func<T, object>>[]? includes)
    {
        var query = _context.Set<T>().AsQueryable();

        if (includes is not null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        var entity = await query.FirstOrDefaultAsync(e => EF.Property<Guid>(e, "Id") == Id, cancellationToken);

        return entity;
    }

    public async Task<T?> GetByIdAsync(int Id, CancellationToken cancellationToken, params Expression<Func<T, object>>[]? includes)
    {
        var query = _context.Set<T>().AsQueryable();

        if (includes is not null)
        {
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
        }

        var entity = await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == Id, cancellationToken);

        return entity;
    }
}
    