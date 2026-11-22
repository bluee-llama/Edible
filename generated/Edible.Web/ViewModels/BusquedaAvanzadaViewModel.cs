using Edible.Domain.Enums;

namespace Edible.Web.ViewModels;

// ViewModel manual del filtro Receta.busquedaAvanzada (ReadFilter).
// Los valores 0 / "ninguna" / "ninguno" significan "sin filtrar" (igual que en el repositorio).
public sealed class BusquedaAvanzadaViewModel
{
    public DietaEspecial Dieta { get; set; } = DietaEspecial.ninguna;
    public float NotaMinima { get; set; }
    public int TiempoMin { get; set; }
    public int TiempoMax { get; set; }
    public int Dificultad { get; set; }
    public Alergenos AlergenoExcluido { get; set; } = Alergenos.ninguno;
    public int IngredienteId { get; set; }

    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> IngredienteOptions { get; set; } = new();

    // false hasta que el usuario envía el formulario: así se distingue "sin buscar" de "sin resultados".
    public bool Buscado { get; set; }
    public List<RecetaViewModel> Resultados { get; set; } = new();
}
