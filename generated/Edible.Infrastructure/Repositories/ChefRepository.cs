using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Exceptions;

namespace Edible.Infrastructure.Repositories;

public sealed class ChefRepository : RepositoryBase<Chef>, IChefRepository
{
    public ChefRepository(ILogger<ChefRepository>? logger = null)
        : base(logger)
    {
    }

    public ChefRepository(ISession session, ILogger<ChefRepository>? logger = null)
        : base(session, logger)
    {
    }

    public ChefRepository(ISessionFactory sessionFactory, ILogger<ChefRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<Chef>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Chef>().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<Chef?> ReadAsync(object id)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.GetAsync<Chef>(id);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(Chef entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(Chef entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }


}