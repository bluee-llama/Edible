using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Exceptions;

namespace Edible.Infrastructure.Repositories;

public sealed class IngredienteRepository : RepositoryBase<Ingrediente>, IIngredienteRepository
{
    public IngredienteRepository(ILogger<IngredienteRepository>? logger = null)
        : base(logger)
    {
    }

    public IngredienteRepository(ISession session, ILogger<IngredienteRepository>? logger = null)
        : base(session, logger)
    {
    }

    public IngredienteRepository(ISessionFactory sessionFactory, ILogger<IngredienteRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<Ingrediente>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Ingrediente>().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<Ingrediente?> ReadAsync(object id)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.GetAsync<Ingrediente>(id);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(Ingrediente entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(Ingrediente entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }


}