using Microsoft.EntityFrameworkCore;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Infrastructure.Data;

namespace Edible.Infrastructure.Queries;

/// <summary>Implementación EF Core de <see cref="IRecetasCompartidasQuery"/> (solo lectura, sin seguimiento).</summary>
public sealed class RecetasCompartidasQuery : IRecetasCompartidasQuery
{
    private readonly EdibleDbContext _dbContext;

    public RecetasCompartidasQuery(EdibleDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<IReadOnlyList<Receta>> ReadRecetasSugeridasAsync(int usuarioId)
    {
        return await _dbContext.Set<Usuario>()
            .AsNoTracking()
            .Where(usuario => usuario.Id == usuarioId)
            .SelectMany(usuario => usuario.RecetasSugeridas)
            .Include(receta => receta.Chef)
            .ToListAsync();
    }
}
