using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Data;
using Edible.Infrastructure.Exceptions;

namespace Edible.Infrastructure.Repositories;

public sealed class LineaRecetaRepository : RepositoryBase<LineaReceta>, ILineaRecetaRepository
{
    public LineaRecetaRepository(ILogger<LineaRecetaRepository>? logger = null)
        : base(logger)
    {
    }

    public LineaRecetaRepository(EdibleDbContext dbContext, ILogger<LineaRecetaRepository>? logger = null)
        : base(dbContext, logger)
    {
    }

    public async Task<IReadOnlyList<LineaReceta>> ReadAllAsync()
    {
        try
        {
            return await DbContext.Set<LineaReceta>().Include(entity => entity.Receta).Include(entity => entity.IngredienteCatalogo).ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<LineaReceta?> ReadAsync(object id)
    {
        try
        {
            var entity = await DbContext.Set<LineaReceta>().FindAsync([id]);
                if (entity is not null) await DbContext.Entry(entity).Reference(item => item.Receta).LoadAsync();
                if (entity is not null) await DbContext.Entry(entity).Reference(item => item.IngredienteCatalogo).LoadAsync();
            return entity;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(LineaReceta entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            async dbContext =>
            {
                var set = dbContext.Set<LineaReceta>();
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

    public async Task DeleteAsync(LineaReceta entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            dbContext =>
            {
                dbContext.Set<LineaReceta>().Remove(entity);
                return Task.CompletedTask;
            },
            $"Error deleting {EntityName} entity.");
    }

    private static bool HasDefaultKey(LineaReceta entity) =>
        EqualityComparer<int>.Default.Equals(entity.Id, default);

    private static Task<bool> EntityExistsAsync(DbSet<LineaReceta> set, LineaReceta entity) =>
        set.AsNoTracking().AnyAsync(existing => existing.Id == entity.Id);

}