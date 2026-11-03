using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Application.Abstractions;

public interface IIngredienteNeveraRepository : IRepository<IngredienteNevera>
{
    Task<IReadOnlyList<IngredienteNevera>> FiltrarNeveraAsync(int p_nevera_oid, TipoIngrediente p_tipo, DateTime p_fechaLimiteCerca, DateTime p_fechaActual, EstadoCaducidad p_estado);
}
