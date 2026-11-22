// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(IngredienteNeveraController_manual_usings) ENABLED START*/
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
/*PROTECTED REGION ID(IngredienteNeveraController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION END*/
public partial class IngredienteNeveraController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteNeveraController_ComprobarCaducidad_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ComprobarCaducidad(int p_oid)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_ComprobarCaducidad_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _ingredienteNeveraService.ComprobarCaducidadAsync(p_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }
}
