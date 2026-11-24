using System.ComponentModel.DataAnnotations;
using Edible.Domain.Enums;

namespace Edible.Web.ViewModels;

// ViewModel manual del filtro IngredienteNevera.filtrarNevera (ReadFilter).
// Los valores TipoIngrediente.ninguno y EstadoCaducidad.ninguno significan "sin filtrar por ese criterio".
public sealed class FiltrarNeveraViewModel
{
    public int NeveraId { get; set; }
    public TipoIngrediente Tipo { get; set; } = TipoIngrediente.ninguno;

    [DataType(DataType.Date)]
    public DateTime FechaActual { get; set; } = DateTime.Today;

    [DataType(DataType.Date)]
    public DateTime FechaLimiteCerca { get; set; } = DateTime.Today.AddDays(3);

    public EstadoCaducidad Estado { get; set; } = EstadoCaducidad.ninguno;

    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> NeveraOptions { get; set; } = new();

    // false hasta que el usuario envía el formulario GET con parámetros.
    public bool Buscado { get; set; }
    public List<IngredienteNeveraViewModel> Resultados { get; set; } = new();
}
