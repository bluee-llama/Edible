using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Data;
using Edible.Infrastructure.Exceptions;

namespace Edible.Infrastructure.Repositories;

public sealed class NeveraRepository : RepositoryBase<Nevera>, INeveraRepository
{
    public NeveraRepository(ILogger<NeveraRepository>? logger = null)
        : base(logger)
    {
    }

    public NeveraRepository(EdibleDbContext dbContext, ILogger<NeveraRepository>? logger = null)
        : base(dbContext, logger)
    {
    }

    public async Task<IReadOnlyList<Nevera>> ReadAllAsync()
    {
        try
        {
            return await DbContext.Set<Nevera>().Include(entity => entity.Usuario).Include(entity => entity.IngredienteEnLaLista).Include(entity => entity.IngredienteEnLaNevera).Include("IngredienteEnLaLista.ListaCompra").Include("IngredienteEnLaLista.IngredienteCatalogo").Include("IngredienteEnLaNevera.IngredienteCatalogo").Include("IngredienteEnLaNevera.Receta").ToListAsync();
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
            var entity = await DbContext.Set<Nevera>().FindAsync([id]);
                if (entity is not null) await DbContext.Entry(entity).Reference(item => item.Usuario).LoadAsync();
                if (entity is not null) await DbContext.Entry(entity).Collection(item => item.IngredienteEnLaLista).LoadAsync();
                if (entity is not null) await DbContext.Entry(entity).Collection(item => item.IngredienteEnLaNevera).LoadAsync();
                if (entity is not null) foreach (var child in entity.IngredienteEnLaLista) await DbContext.Entry(child).Reference(item => item.ListaCompra).LoadAsync();
                if (entity is not null) foreach (var child in entity.IngredienteEnLaLista) await DbContext.Entry(child).Reference(item => item.IngredienteCatalogo).LoadAsync();
                if (entity is not null) foreach (var child in entity.IngredienteEnLaNevera) await DbContext.Entry(child).Reference(item => item.IngredienteCatalogo).LoadAsync();
                if (entity is not null) foreach (var child in entity.IngredienteEnLaNevera) await DbContext.Entry(child).Reference(item => item.Receta).LoadAsync();
            return entity;
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
            async dbContext =>
            {
                var set = dbContext.Set<Nevera>();
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

    public async Task DeleteAsync(Nevera entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            dbContext =>
            {
                dbContext.Set<Nevera>().Remove(entity);
                return Task.CompletedTask;
            },
            $"Error deleting {EntityName} entity.");
    }

    private static bool HasDefaultKey(Nevera entity) =>
        EqualityComparer<int>.Default.Equals(entity.Id, default);

    private static Task<bool> EntityExistsAsync(DbSet<Nevera> set, Nevera entity) =>
        set.AsNoTracking().AnyAsync(existing => existing.Id == entity.Id);

}