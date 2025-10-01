using Microsoft.EntityFrameworkCore.Storage;
using Verimood.Warehouse.Domain.Entities;
using Verimood.Warehouse.Domain.Repositories;

namespace Verimood.Warehouse.Domain.Uow;

public interface IUnitOfWork
{
    Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
}
