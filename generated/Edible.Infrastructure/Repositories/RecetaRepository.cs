using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Data;
using Edible.Infrastructure.Exceptions;

namespace Edible.Infrastructure.Repositories;

public sealed class RecetaRepository : RepositoryBase<Receta>, IRecetaRepository
{
    public RecetaRepository(ILogger<RecetaRepository>? logger = null)
        : base(logger)
    {
    }

    public RecetaRepository(EdibleDbContext dbContext, ILogger<RecetaRepository>? logger = null)
        : base(dbContext, logger)
    {
    }

    public async Task<IReadOnlyList<Receta>> ReadAllAsync()
    {
        try
        {
            return await DbContext.Set<Receta>().Include(entity => entity.Chef).Include(entity => entity.Pasos).Include(entity => entity.Lineas).Include("Lineas.IngredienteCatalogo").ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<Receta?> ReadAsync(object id)
    {
        try
        {
            var entity = await DbContext.Set<Receta>().FindAsync([id]);
                if (entity is not null) await DbContext.Entry(entity).Reference(item => item.Chef).LoadAsync();
                if (entity is not null) await DbContext.Entry(entity).Collection(item => item.Pasos).LoadAsync();
                if (entity is not null) await DbContext.Entry(entity).Collection(item => item.Lineas).LoadAsync();
                if (entity is not null) foreach (var child in entity.Lineas) await DbContext.Entry(child).Reference(item => item.IngredienteCatalogo).LoadAsync();
                if (entity is not null) await DbContext.Entry(entity).Collection(item => item.UsuariosSugeridos).LoadAsync();
            return entity;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(Receta entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            async dbContext =>
            {
                var set = dbContext.Set<Receta>();
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

    public async Task DeleteAsync(Receta entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            dbContext =>
            {
                dbContext.Set<Receta>().Remove(entity);
                return Task.CompletedTask;
            },
            $"Error deleting {EntityName} entity.");
    }

    private static bool HasDefaultKey(Receta entity) =>
        EqualityComparer<int>.Default.Equals(entity.Id, default);

    private static Task<bool> EntityExistsAsync(DbSet<Receta> set, Receta entity) =>
        set.AsNoTracking().AnyAsync(existing => existing.Id == entity.Id);

    public async Task<IReadOnlyList<Receta>> BusquedaAvanzadaAsync(DietaEspecial p_dieta, float p_notaMinima, int p_tiempoMin, int p_tiempoMax, int p_dificultad, Alergenos p_alergenoExcluido, int p_ingrediente_oid)
    {
        try
        {
            var results = await DbContext.Set<Receta>()
                .Include(entity => entity.Chef)
                .Include(entity => entity.Lineas)
                    .ThenInclude(l => l.IngredienteCatalogo)
                .Where(entity => (p_dieta == DietaEspecial.ninguna || entity.DietaEspecial == p_dieta) && entity.ValoracionMedia >= p_notaMinima && entity.TiempoRealizacion >= p_tiempoMin && (p_tiempoMax <= 0 || entity.TiempoRealizacion <= p_tiempoMax) && (p_dificultad <= 0 || entity.Dificultad == p_dificultad) && (p_ingrediente_oid <= 0 || entity.Lineas.Any(l => l.IngredienteCatalogo.Id == p_ingrediente_oid)))
                .ToListAsync();

            if (p_alergenoExcluido != Alergenos.ninguno)
            {
                results = results
                    .Where(entity => !entity.Lineas.Any(l => l.IngredienteCatalogo.Alergenos.Contains(p_alergenoExcluido)))
                    .ToList();
            }

            return results;
        }
        catch (DataException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Receta.busquedaAvanzada.");
            throw new DataException("Error executing ReadFilter Receta.busquedaAvanzada.", ex);
        }
    }

    public async Task<IReadOnlyList<Receta>> BuscarNombreAsync(string p_nombre)
    {
        try
        {
            var filtro = (p_nombre ?? string.Empty).Trim().ToLower();
            return await DbContext.Set<Receta>()
                .Include(entity => entity.Chef)
                .Where(entity => entity.Nombre.ToLower().Contains(filtro))
                .ToListAsync();
        }
        catch (DataException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Receta.buscarNombre.");
            throw new DataException("Error executing ReadFilter Receta.buscarNombre.", ex);
        }
    }

    public async Task<IReadOnlyList<Receta>> RecetasPorNeveraAsync(int p_nevera_oid)
    {
        try
        {
            return await DbContext.Set<Receta>()
                .Include(entity => entity.Chef)
                .Where(entity => entity.Lineas.All(linea => linea.IngredienteCatalogo.IngredienteEnNevera.Any(nev => nev.Nevera.Id == p_nevera_oid && nev.Cantidad >= linea.CantidadNecesaria)))
                .ToListAsync();
        }
        catch (DataException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Receta.recetasPorNevera.");
            throw new DataException("Error executing ReadFilter Receta.recetasPorNevera.", ex);
        }
    }
}
