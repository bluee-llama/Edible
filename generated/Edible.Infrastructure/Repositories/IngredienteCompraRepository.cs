using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Exceptions;

namespace Edible.Infrastructure.Repositories;

public sealed class IngredienteCompraRepository : RepositoryBase<IngredienteCompra>, IIngredienteCompraRepository
{
    public IngredienteCompraRepository(ILogger<IngredienteCompraRepository>? logger = null)
        : base(logger)
    {
    }

    public IngredienteCompraRepository(ISession session, ILogger<IngredienteCompraRepository>? logger = null)
        : base(session, logger)
    {
    }

    public IngredienteCompraRepository(ISessionFactory sessionFactory, ILogger<IngredienteCompraRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<IngredienteCompra>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<IngredienteCompra>().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<IngredienteCompra?> ReadAsync(object id)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.GetAsync<IngredienteCompra>(id);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(IngredienteCompra entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(IngredienteCompra entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }


}