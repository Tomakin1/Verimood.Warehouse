using Microsoft.EntityFrameworkCore.Storage;
using Verimood.Warehouse.Domain.Uow;
using Verimood.Warehouse.Persistence.Context;

namespace Verimood.Warehouse.Persistence.Uow;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.BeginTransactionAsync(cancellationToken);
    }
}
