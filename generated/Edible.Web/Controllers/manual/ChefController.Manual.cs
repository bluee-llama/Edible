// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(ChefController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;
using Edible.Web.Utils.Security;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

// La autorización de clase está en ChefController.cs (región class_attributes) y aplica a ambos parciales.
/*PROTECTED REGION ID(ChefController_manual_class_attributes) ENABLED START*/
// La autorización de clase está en ChefController.cs (región class_attributes) y aplica a ambos parciales.
/*PROTECTED REGION END*/
public partial class ChefController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ChefController_PonerPublico_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> PonerPublico(int p_oid)
    {
    /*PROTECTED REGION ID(ChefController_PonerPublico_POST_body) ENABLED START*/

        // Solo el propio usuario o un Administrador pueden cambiar la visibilidad.
        if (!User.CanManageUsuario(p_oid)) return Forbid();

        try
        {
            await _usuarioService.PonerPublicoAsync(p_oid);
            TempData["Success"] = "El perfil ahora es público.";
        }
        catch (ModelException error)
        {
            TempData["Error"] = error.InnerException?.Message ?? error.Message;
        }

        // PRG: Post -> Redirect -> Get
        return RedirectToAction(nameof(Details), new { id = p_oid });
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ChefController_PonerPrivado_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> PonerPrivado(int p_oid)
    {
    /*PROTECTED REGION ID(ChefController_PonerPrivado_POST_body) ENABLED START*/

        // Solo el propio usuario o un Administrador pueden cambiar la visibilidad.
        if (!User.CanManageUsuario(p_oid)) return Forbid();

        try
        {
            await _usuarioService.PonerPrivadoAsync(p_oid);
            TempData["Success"] = "El perfil ahora es privado.";
        }
        catch (ModelException error)
        {
            TempData["Error"] = error.InnerException?.Message ?? error.Message;
        }

        // PRG: Post -> Redirect -> Get
        return RedirectToAction(nameof(Details), new { id = p_oid });
    /*PROTECTED REGION END*/
    }
}
