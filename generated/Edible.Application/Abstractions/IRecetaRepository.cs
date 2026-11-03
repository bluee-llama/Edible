using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Application.Abstractions;

public interface IRecetaRepository : IRepository<Receta>
{
    Task<IReadOnlyList<Receta>> BusquedaAvanzadaAsync(DietaEspecial p_dieta, float p_notaMinima, int p_tiempoMin, int p_tiempoMax, int p_dificultad, Alergenos p_alergenoExcluido, int p_ingrediente_oid);
    Task<IReadOnlyList<Receta>> BuscarNombreAsync(string p_nombre);
    Task<IReadOnlyList<Receta>> RecetasPorNeveraAsync(int p_nevera_oid);
}
