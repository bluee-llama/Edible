using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Exceptions;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace Edible.Infrastructure.Repositories;

public sealed class RecetaRepository : RepositoryBase<Receta>, IRecetaRepository
{
    public RecetaRepository(ILogger<RecetaRepository>? logger = null)
        : base(logger)
    {
    }

    public RecetaRepository(ISession session, ILogger<RecetaRepository>? logger = null)
        : base(session, logger)
    {
    }

    public RecetaRepository(ISessionFactory sessionFactory, ILogger<RecetaRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<Receta>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Receta>().ToListAsync();
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
            using var context = OpenSession();
            return await context.Session.GetAsync<Receta>(id);
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
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(Receta entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }

    public async Task<IReadOnlyList<Receta>> BusquedaAvanzadaAsync(DietaEspecial p_dieta, float p_notaMinima, int p_tiempoMin, int p_tiempoMax, int p_dificultad, Alergenos p_alergenoExcluido, int p_ingrediente_oid)
    {
        /*PROTECTED REGION ID(Edible.Infrastructure.Repositories_Receta_busquedaAvanzada) ENABLED START*/
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Receta>()
                .Where(entity => (p_dieta == DietaEspecial.ninguna || entity.DietaEspecial == p_dieta) && entity.ValoracionMedia >= p_notaMinima && entity.TiempoRealizacion >= p_tiempoMin && (p_tiempoMax <= 0 || entity.TiempoRealizacion <= p_tiempoMax) && (p_dificultad <= 0 || entity.Dificultad == p_dificultad) && (p_alergenoExcluido == Alergenos.ninguno || !entity.Lineas.Any(l => l.IngredienteCatalogo.Alergenos.Contains(p_alergenoExcluido))) && (p_ingrediente_oid <= 0 || entity.Lineas.Any(l => l.IngredienteCatalogo.Id == p_ingrediente_oid))).ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Receta.busquedaAvanzada.");
            throw new DataException("Error executing ReadFilter Receta.busquedaAvanzada.", ex);
        }
        /*PROTECTED REGION END*/
    }

    public async Task<IReadOnlyList<Receta>> BuscarNombreAsync(string p_nombre)
    {
        /*PROTECTED REGION ID(Edible.Infrastructure.Repositories_Receta_buscarNombre) ENABLED START*/
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Receta>()
                .Where(entity => entity.Nombre.ToLower().Contains(p_nombre.ToLower()))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Receta.buscarNombre.");
            throw new DataException("Error executing ReadFilter Receta.buscarNombre.", ex);
        }
        /*PROTECTED REGION END*/
    }

    public async Task<IReadOnlyList<Receta>> RecetasPorNeveraAsync(int p_nevera_oid)
    {
        /*PROTECTED REGION ID(Edible.Infrastructure.Repositories_Receta_recetasPorNevera) ENABLED START*/
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<Receta>()
                .Where(entity => entity.Lineas.All(linea => 
                    linea.IngredienteCatalogo.IngredienteEnNevera.Any(nev => 
                        nev.Nevera.Id == p_nevera_oid && nev.Cantidad >= linea.CantidadNecesaria)))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter Receta.recetasPorNevera.");
            throw new DataException("Error executing ReadFilter Receta.recetasPorNevera.", ex);
        }
        /*PROTECTED REGION END*/
    }
}