// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(IngredienteCompraController_manual_usings) ENABLED START*/
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
/*PROTECTED REGION ID(IngredienteCompraController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION END*/
public partial class IngredienteCompraController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteCompraController_MarcarComprado_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> MarcarComprado(int p_oid)
    {
    /*PROTECTED REGION ID(IngredienteCompraController_MarcarComprado_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _ingredienteCompraService.MarcarCompradoAsync(p_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteCompraController_DesmarcarComprado_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DesmarcarComprado(int p_oid)
    {
    /*PROTECTED REGION ID(IngredienteCompraController_DesmarcarComprado_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _ingredienteCompraService.DesmarcarCompradoAsync(p_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }
}
