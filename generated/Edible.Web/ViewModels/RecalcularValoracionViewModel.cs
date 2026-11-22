using System.ComponentModel.DataAnnotations;

namespace Edible.Web.ViewModels;

// ViewModel manual del formulario de la operación Comentario.recalcularValoracionMedia.
// Los nombres de los campos del formulario son los parámetros p_* de la acción POST.
public sealed class RecalcularValoracionViewModel
{
    public int User_id { get; set; }

    [Required(ErrorMessage = "Debes indicar el título.")]
    public string Titulo { get; set; } = string.Empty;

    [Range(1, 5, ErrorMessage = "La valoración debe estar entre 1 y 5.")]
    public int Valoracion { get; set; } = 5;

    public int? ComentadorId { get; set; }
    public int? RecetaId { get; set; }

    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ComentadorOptions { get; set; } = new();
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> RecetaOptions { get; set; } = new();

    // Valoración media persistida de cada receta, para comprobar el efecto de la operación.
    public List<RecetaResumen> Recetas { get; set; } = new();

    public sealed record RecetaResumen(int Id, string Nombre, float ValoracionMedia);
}
