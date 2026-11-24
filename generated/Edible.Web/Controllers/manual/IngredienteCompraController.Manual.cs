// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(IngredienteCompraController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Application.Exceptions;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION ID(IngredienteCompraController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// La autorización de clase ([Authorize(Roles = "Usuario,Chef,Administrador")]) está en IngredienteCompraController.cs.
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

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Identificador de ingrediente de compra no válido.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await _ingredienteCompraService.MarcarCompradoAsync(p_oid);
            TempData["Success"] = "Ingrediente marcado como comprado.";
        }
        catch (ModelException error)
        {
            TempData["Error"] = error.InnerException?.Message ?? error.Message;
        }

        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteCompraController_DesmarcarComprado_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DesmarcarComprado(int p_oid)
    {
    /*PROTECTED REGION ID(IngredienteCompraController_DesmarcarComprado_POST_body) ENABLED START*/

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Identificador de ingrediente de compra no válido.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            await _ingredienteCompraService.DesmarcarCompradoAsync(p_oid);
            TempData["Success"] = "Ingrediente desmarcado como pendiente de compra.";
        }
        catch (ModelException error)
        {
            TempData["Error"] = error.InnerException?.Message ?? error.Message;
        }

        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }
}
