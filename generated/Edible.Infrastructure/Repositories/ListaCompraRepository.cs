using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Exceptions;

namespace Edible.Infrastructure.Repositories;

public sealed class ListaCompraRepository : RepositoryBase<ListaCompra>, IListaCompraRepository
{
    public ListaCompraRepository(ILogger<ListaCompraRepository>? logger = null)
        : base(logger)
    {
    }

    public ListaCompraRepository(ISession session, ILogger<ListaCompraRepository>? logger = null)
        : base(session, logger)
    {
    }

    public ListaCompraRepository(ISessionFactory sessionFactory, ILogger<ListaCompraRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<ListaCompra>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<ListaCompra>().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<ListaCompra?> ReadAsync(object id)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.GetAsync<ListaCompra>(id);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(ListaCompra entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(ListaCompra entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }


}