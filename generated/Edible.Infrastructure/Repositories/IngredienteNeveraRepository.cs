using Microsoft.Extensions.Logging;
using NHibernate;
using NHibernate.Linq;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Exceptions;

namespace Edible.Infrastructure.Repositories;

public sealed class IngredienteNeveraRepository : RepositoryBase<IngredienteNevera>, IIngredienteNeveraRepository
{
    public IngredienteNeveraRepository(ILogger<IngredienteNeveraRepository>? logger = null)
        : base(logger)
    {
    }

    public IngredienteNeveraRepository(ISession session, ILogger<IngredienteNeveraRepository>? logger = null)
        : base(session, logger)
    {
    }

    public IngredienteNeveraRepository(ISessionFactory sessionFactory, ILogger<IngredienteNeveraRepository>? logger = null)
        : base(sessionFactory, logger)
    {
    }

    public async Task<IReadOnlyList<IngredienteNevera>> ReadAllAsync()
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<IngredienteNevera>().ToListAsync();
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
            using var context = OpenSession();
            return await context.Session.GetAsync<IngredienteNevera>(id);
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
            session => session.SaveOrUpdateAsync(entity),
            $"Error saving {EntityName} entity.");
    }

    public async Task DeleteAsync(IngredienteNevera entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            session => session.DeleteAsync(entity),
            $"Error deleting {EntityName} entity.");
    }

    public async Task<IReadOnlyList<IngredienteNevera>> FiltrarNeveraAsync(int p_nevera_oid, TipoIngrediente p_tipo, DateTime p_fechaLimiteCerca, DateTime p_fechaActual, EstadoCaducidad p_estado)
    {
        try
        {
            using var context = OpenSession();
            return await context.Session.Query<IngredienteNevera>()
                .Where(entity => entity.Nevera.Id == p_nevera_oid && (p_tipo == TipoIngrediente.ninguno || entity.IngredienteCatalogo.Tipo == p_tipo) && (p_estado == EstadoCaducidad.ninguno || (p_estado == EstadoCaducidad.perfecto && entity.FechaCaducidad > p_fechaLimiteCerca) || (p_estado == EstadoCaducidad.cerca && entity.FechaCaducidad <= p_fechaLimiteCerca && entity.FechaCaducidad >= p_fechaActual) || (p_estado == EstadoCaducidad.caducado && entity.FechaCaducidad < p_fechaActual)))
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error executing ReadFilter IngredienteNevera.filtrarNevera.");
            throw new DataException("Error executing ReadFilter IngredienteNevera.filtrarNevera.", ex);
        }
    }
}