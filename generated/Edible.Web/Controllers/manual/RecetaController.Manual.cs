// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(RecetaController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION ID(RecetaController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION END*/
public partial class RecetaController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(RecetaController_Compartir_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Compartir(int p_oid)
    {
    /*PROTECTED REGION ID(RecetaController_Compartir_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _recetaService.CompartirAsync(p_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(RecetaController_ComprobarIngredientesDisponibles_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ComprobarIngredientesDisponibles(string p_nombre, string p_descripcion, global::Edible.Domain.Enums.DietaEspecial p_dietaEspecial, float p_valoracionMedia, int p_chef_oid)
    {
    /*PROTECTED REGION ID(RecetaController_ComprobarIngredientesDisponibles_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await using var unitOfWork = _useCaseUnitOfWorkFactory.Create();
        var useCase = new Edible.Application.UseCases.Receta.RecetaUseCase(unitOfWork);
        var result = await useCase.ComprobarIngredientesDisponiblesAsync(p_nombre, p_descripcion, p_dietaEspecial, p_valoracionMedia, p_chef_oid);
        return Ok(result);
    /*PROTECTED REGION END*/
    }
}
