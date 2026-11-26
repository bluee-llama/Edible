using Edible.Domain.Entities;

namespace Edible.Application.Abstractions;

/// <summary>
/// Consulta de solo lectura (fichero manual, no generado) para obtener las recetas que se han
/// compartido con un usuario (relación N:M Usuario.RecetasSugeridas / Receta.UsuariosSugeridos).
/// Los repositorios generados no cargan esa colección, por eso se expone aparte.
/// </summary>
public interface IRecetasCompartidasQuery
{
    /// <summary>Recetas compartidas con el usuario, con su Chef cargado. Vacío si no hay ninguna.</summary>
    Task<IReadOnlyList<Receta>> ReadRecetasSugeridasAsync(int usuarioId);
}
