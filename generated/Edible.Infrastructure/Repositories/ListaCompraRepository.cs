using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Domain.ValueObjects;
using Edible.Infrastructure.Data;
using Edible.Infrastructure.Exceptions;

namespace Edible.Infrastructure.Repositories;

public sealed class ListaCompraRepository : RepositoryBase<ListaCompra>, IListaCompraRepository
{
    public ListaCompraRepository(ILogger<ListaCompraRepository>? logger = null)
        : base(logger)
    {
    }

    public ListaCompraRepository(EdibleDbContext dbContext, ILogger<ListaCompraRepository>? logger = null)
        : base(dbContext, logger)
    {
    }

    public async Task<IReadOnlyList<ListaCompra>> ReadAllAsync()
    {
        try
        {
            return await DbContext.Set<ListaCompra>().Include(entity => entity.Usuario).Include(entity => entity.IngredienteEnLaLista).Include("IngredienteEnLaLista.Nevera").Include("IngredienteEnLaLista.IngredienteCatalogo").ToListAsync();
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading all {Entity} entities.", EntityName);
            throw new DataException($"Error reading all {EntityName} entities.", ex);
        }
    }

    public async Task<ListaCompra?> ReadAsync(object id)
    {
        try
        {
            var entity = await DbContext.Set<ListaCompra>().FindAsync([id]);
                if (entity is not null) await DbContext.Entry(entity).Reference(item => item.Usuario).LoadAsync();
                if (entity is not null) await DbContext.Entry(entity).Collection(item => item.IngredienteEnLaLista).LoadAsync();
                if (entity is not null) foreach (var child in entity.IngredienteEnLaLista) await DbContext.Entry(child).Reference(item => item.Nevera).LoadAsync();
                if (entity is not null) foreach (var child in entity.IngredienteEnLaLista) await DbContext.Entry(child).Reference(item => item.IngredienteCatalogo).LoadAsync();
            return entity;
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error reading {Entity} entity with id {EntityId}.", EntityName, id);
            throw new DataException($"Error reading {EntityName} entity.", ex);
        }
    }

    public async Task SaveAsync(ListaCompra entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Save",
            async dbContext =>
            {
                var set = dbContext.Set<ListaCompra>();
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

    public async Task DeleteAsync(ListaCompra entity)
    {
        await ExecuteRepositoryOperationInTransactionAsync(
            "Delete",
            dbContext =>
            {
                dbContext.Set<ListaCompra>().Remove(entity);
                return Task.CompletedTask;
            },
            $"Error deleting {EntityName} entity.");
    }

    private static bool HasDefaultKey(ListaCompra entity) =>
        EqualityComparer<int>.Default.Equals(entity.Id, default);

    private static Task<bool> EntityExistsAsync(DbSet<ListaCompra> set, ListaCompra entity) =>
        set.AsNoTracking().AnyAsync(existing => existing.Id == entity.Id);

}