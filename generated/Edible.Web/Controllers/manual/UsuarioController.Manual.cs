// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(UsuarioController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;
using Edible.Web.Utils.Security;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION ID(UsuarioController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION END*/
public partial class UsuarioController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(UsuarioController_PonerPublico_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> PonerPublico(int p_oid)
    {
    /*PROTECTED REGION ID(UsuarioController_PonerPublico_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _usuarioService.PonerPublicoAsync(p_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(UsuarioController_PonerPrivado_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> PonerPrivado(int p_oid)
    {
    /*PROTECTED REGION ID(UsuarioController_PonerPrivado_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await _usuarioService.PonerPrivadoAsync(p_oid);
        return NoContent();
    /*PROTECTED REGION END*/
    }
}
