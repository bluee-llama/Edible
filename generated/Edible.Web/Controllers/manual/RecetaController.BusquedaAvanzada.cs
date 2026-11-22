// Parcial manual independiente: la lógica de presentación de Receta.busquedaAvanzada vive aquí
// para no mezclarla con RecetaController.Manual.cs (Compartir, ComprobarIngredientesDisponibles...).
// La acción GET (RecetaController.BusquedaAvanzada) está en RecetaController.cs, en su región ENABLED,
// y delega en BuildBusquedaAvanzadaAsync.
using Microsoft.AspNetCore.Mvc;
using Edible.Domain.Enums;
using Edible.Web.Assemblers;
using Edible.Web.ViewModels;

namespace Edible.Web.Controllers;

public partial class RecetaController
{
    private async Task<BusquedaAvanzadaViewModel> BuildBusquedaAvanzadaAsync(
        DietaEspecial p_dieta, float p_notaMinima, int p_tiempoMin, int p_tiempoMax,
        int p_dificultad, Alergenos p_alergenoExcluido, int p_ingrediente_oid)
    {
        var model = new BusquedaAvanzadaViewModel
        {
            Dieta = p_dieta,
            NotaMinima = p_notaMinima,
            TiempoMin = p_tiempoMin,
            TiempoMax = p_tiempoMax,
            Dificultad = p_dificultad,
            AlergenoExcluido = p_alergenoExcluido,
            IngredienteId = p_ingrediente_oid,
            // Solo se ejecuta la búsqueda cuando llegan parámetros en la URL (formulario enviado).
            Buscado = Request.Query.Count > 0
        };

        await using (var unitOfWork = _formUnitOfWorkFactory.Create())
        {
            model.IngredienteOptions = (await unitOfWork.IngredienteRepository.ReadAllAsync())
                .OrderBy(i => i.Nombre)
                .Select(i => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = Convert.ToString(i.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
                    Text = i.Nombre
                }).ToList();
        }

        if (model.Buscado)
        {
            if (p_tiempoMax > 0 && p_tiempoMin > p_tiempoMax)
                ModelState.AddModelError(nameof(model.TiempoMin), "El tiempo mínimo no puede ser mayor que el máximo.");
            else
            {
                var result = await _recetaService.BusquedaAvanzadaAsync(
                    p_dieta, p_notaMinima, p_tiempoMin, p_tiempoMax, p_dificultad, p_alergenoExcluido, p_ingrediente_oid);
                model.Resultados = result.Select(RecetaAssembler.ToViewModel).ToList();
            }
        }
        return model;
    }
}
