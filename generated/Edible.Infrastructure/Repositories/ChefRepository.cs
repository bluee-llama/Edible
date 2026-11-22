using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Data;
using Edible.Infrastructure.Exceptions;

namespace Edible.Infrastructure.Repositories;

public sealed class ChefRepository : RepositoryBase<Chef>, IChefRepository
{
    public ChefRepository(ILogger<ChefRepository>? logger = null)
        : base(logger)
    {
    }

    public ChefRepository(EdibleDbContext dbContext, ILogger<ChefRepository>? logger = null)
        : base(dbContext, logger)
    {
    }

    public async Task<IReadOnlyList<Chef>> ReadAllAsync()
    {
        try
        {
            return await DbContext.Set<Chef>().Include(entity => entity.Nevera).Include(entity => entity.ListaCompra).ToListAsync();
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
            var entity = await DbContext.Set<Chef>().FindAsync([id]);
                if (entity is not null) await DbContext.Entry(entity).Reference(item => item.Nevera).LoadAsync();
                if (entity is not null) await DbContext.Entry(entity).Reference(item => item.ListaCompra).LoadAsync();
            return entity;
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
            async dbContext =>
            {
                var set = dbContext.Set<Chef>();
                var entry = dbContext.Entry(entity);
                if (entry.State == EntityState.Detached)
                {
                    if (HasDefaultKey(entity))
                    {
                        set.Add(entity);
                    }
                    else
                    {
                        var exists = await EntityExistsAsync(set, entity);
                        if (exists)
                        {
                            set.Update(entity);
                        }
                        else
                        {
                            set.Add(entity);
                        }
                    }
                }
                else
                {
                    set.Update(entity);
                }
            },
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(Chef entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            dbContext =>
            {
                dbContext.Set<Chef>().Remove(entity);
                return Task.CompletedTask;
            },
            $"Error deleting {EntityName} entity.");
    }

    private static bool HasDefaultKey(Chef entity) => false;

    private static Task<bool> EntityExistsAsync(DbSet<Chef> set, Chef entity) =>
        Task.FromResult(true);

}