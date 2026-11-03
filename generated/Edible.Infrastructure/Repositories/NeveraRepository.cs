using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Exceptions;

namespace Edible.Infrastructure.Repositories;

public sealed class NeveraRepository : RepositoryBase<Nevera>, INeveraRepository
{
    public NeveraRepository(ILogger<NeveraRepository>? logger = null)
        : base(logger)
    {
    }

    public NeveraRepository(ISession session, ILogger<NeveraRepository>? logger = null)
        : base(session, logger)
    {
    }

    public NeveraRepository(ISessionFactory sessionFactory, ILogger<NeveraRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<Nevera>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Nevera>().ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<Nevera?> ReadAsync(object id)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.GetAsync<Nevera>(id);
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(Nevera entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(Nevera entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }


}