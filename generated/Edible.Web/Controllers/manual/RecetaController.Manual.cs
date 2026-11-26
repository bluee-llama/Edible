// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(RecetaController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
using Edible.Application.Exceptions;
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

        // El destinatario llega en el formulario (p_usuario_oid); la firma generada solo declara p_oid.
        if (!int.TryParse(Request.Form["p_usuario_oid"].ToString(), System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var usuarioOid) || usuarioOid <= 0)
        {
            TempData["Error"] = "Selecciona el usuario con el que quieres compartir la receta.";
            return RedirectToAction(nameof(Details), new { id = p_oid });
        }

        try
        {
            await _recetaService.CompartirAsync(p_oid, usuarioOid);
            TempData["Exito"] = "Receta compartida correctamente.";
        }
        catch (ModelException error)
        {
            TempData["Error"] = error.Message;
        }
        return RedirectToAction(nameof(Details), new { id = p_oid });
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(RecetaController_ComprobarIngredientesDisponibles_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ComprobarIngredientesDisponibles(string p_nombre, string p_descripcion, global::Edible.Domain.Enums.DietaEspecial p_dietaEspecial, float p_valoracionMedia, int p_chef_oid)
    {
    /*PROTECTED REGION ID(RecetaController_ComprobarIngredientesDisponibles_POST_body) ENABLED START*/

        // Solo se valida lo que usa el caso de uso: nombre de la receta y chef dueño de la nevera.
        if (string.IsNullOrWhiteSpace(p_nombre) || p_chef_oid <= 0)
        {
            TempData["Error"] = "Selecciona una receta para comprobar.";
            return RedirectToAction(nameof(ComprobarIngredientes));
        }
        if (!RecetaWebSupport.CanUseUser(User, p_chef_oid)) return Forbid();

        try
        {
            await using var unitOfWork = _useCaseUnitOfWorkFactory.Create();
            var useCase = new Edible.Application.UseCases.Receta.RecetaUseCase(unitOfWork);
            var result = await useCase.ComprobarIngredientesDisponiblesAsync(p_nombre, p_descripcion, p_dietaEspecial, p_valoracionMedia, p_chef_oid);
            TempData["Exito"] = $"Hay stock suficiente en la nevera para preparar «{result.Nombre}».";
        }
        catch (ModelException error)
        {
            // El caso de uso envuelve el motivo real (qué falta) en la excepción interna.
            TempData["Error"] = error.InnerException is ModelException detalle
                ? detalle.Message
                : "No se ha podido comprobar la receta.";
        }
        return RedirectToAction(nameof(ComprobarIngredientes));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ComprobarMiNevera(int id)
    {
        var receta = await _recetaService.ReadOIDAsync(id);
        if (receta is null) return NotFound();

        if (RecetaWebSupport.CurrentUserId(User) is null)
            return Forbid();

        var puedeCocinarse = await RecetaWebSupport.PuedeCocinarseConMiNeveraAsync(
            _formUnitOfWorkFactory, _recetaService, User, id);
        TempData[puedeCocinarse is true ? "Exito" : "Error"] = puedeCocinarse is null
            ? "No tienes una nevera asociada a tu cuenta."
            : puedeCocinarse.Value
                ? $"Tienes ingredientes suficientes para preparar «{receta.Nombre}»."
                : $"Te faltan ingredientes para preparar «{receta.Nombre}».";
        return RedirectToAction(nameof(Details), new { id });
    }

    /*PROTECTED REGION ID(RecetaController_manual_custom) ENABLED START*/
    /// <summary>Formulario GET de ComprobarIngredientesDisponibles (CustomTransaction); la comprobación va por POST + PRG.</summary>
    [HttpGet]
    public async Task<IActionResult> ComprobarIngredientes()
    {
        var opciones = await RecetaWebSupport.LoadRecetaChecksAsync(_formUnitOfWorkFactory, User);
        return View(opciones);
    }
    /*PROTECTED REGION END*/
}