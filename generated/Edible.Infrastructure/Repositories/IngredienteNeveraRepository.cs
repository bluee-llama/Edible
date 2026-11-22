using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Data;
using Edible.Infrastructure.Exceptions;

namespace Edible.Infrastructure.Repositories;

public sealed class IngredienteNeveraRepository : RepositoryBase<IngredienteNevera>, IIngredienteNeveraRepository
{
    public IngredienteNeveraRepository(ILogger<IngredienteNeveraRepository>? logger = null)
        : base(logger)
    {
    }

    public IngredienteNeveraRepository(EdibleDbContext dbContext, ILogger<IngredienteNeveraRepository>? logger = null)
        : base(dbContext, logger)
    {
    }

    public async Task<IReadOnlyList<IngredienteNevera>> ReadAllAsync()
    {
        try
        {
            return await DbContext.Set<IngredienteNevera>().Include(entity => entity.IngredienteCatalogo).Include(entity => entity.Nevera).Include(entity => entity.Receta).ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<IngredienteNevera?> ReadAsync(object id)
    {
        try
        {
            var entity = await DbContext.Set<IngredienteNevera>().FindAsync([id]);
                if (entity is not null) await DbContext.Entry(entity).Reference(item => item.IngredienteCatalogo).LoadAsync();
                if (entity is not null) await DbContext.Entry(entity).Reference(item => item.Nevera).LoadAsync();
                if (entity is not null) await DbContext.Entry(entity).Reference(item => item.Receta).LoadAsync();
            return entity;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(IngredienteNevera entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            async dbContext =>
            {
                var set = dbContext.Set<IngredienteNevera>();
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

    public async Task DeleteAsync(IngredienteNevera entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            dbContext =>
            {
                dbContext.Set<IngredienteNevera>().Remove(entity);
                return Task.CompletedTask;
            },
            $"Error deleting {EntityName} entity.");
    }

    private static bool HasDefaultKey(IngredienteNevera entity) =>
        EqualityComparer<int>.Default.Equals(entity.Id, default);

    private static Task<bool> EntityExistsAsync(DbSet<IngredienteNevera> set, IngredienteNevera entity) =>
        set.AsNoTracking().AnyAsync(existing => existing.Id == entity.Id);
    public async Task<IReadOnlyList<IngredienteNevera>> FiltrarNeveraAsync(int p_nevera_oid, TipoIngrediente p_tipo, DateTime p_fechaLimiteCerca, DateTime p_fechaActual, EstadoCaducidad p_estado)
    {
        try
        {
            return await DbContext.Set<IngredienteNevera>()
                .Where(entity => entity.Nevera.Id == p_nevera_oid && (p_tipo == TipoIngrediente.ninguno || entity.IngredienteCatalogo.Tipo == p_tipo) && (p_estado == EstadoCaducidad.ninguno || (p_estado == EstadoCaducidad.perfecto && entity.FechaCaducidad > p_fechaLimiteCerca) || (p_estado == EstadoCaducidad.cerca && entity.FechaCaducidad <= p_fechaLimiteCerca && entity.FechaCaducidad >= p_fechaActual) || (p_estado == EstadoCaducidad.caducado && entity.FechaCaducidad < p_fechaActual)))
                .ToListAsync();
        }
        catch (DataException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter IngredienteNevera.filtrarNevera.");
            throw new DataException("Error executing ReadFilter IngredienteNevera.filtrarNevera.", ex);
        }
    }
}