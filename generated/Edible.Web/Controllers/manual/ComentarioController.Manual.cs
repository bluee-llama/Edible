// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(ComentarioController_manual_usings) ENABLED START*/
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
/*PROTECTED REGION ID(ComentarioController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION END*/
public partial class ComentarioController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ComentarioController_RecalcularValoracionMedia_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> RecalcularValoracionMedia(int p_user_id, string p_titulo, int p_valoracion, int p_comentador_oid, int p_roleB_oid)
    {
    /*PROTECTED REGION ID(ComentarioController_RecalcularValoracionMedia_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await using var unitOfWork = _useCaseUnitOfWorkFactory.Create();
        var useCase = new Edible.Application.UseCases.Comentario.ComentarioUseCase(unitOfWork);
        var result = await useCase.RecalcularValoracionMediaAsync(p_user_id, p_titulo, p_valoracion, p_comentador_oid, p_roleB_oid);
        return Ok(result);
    /*PROTECTED REGION END*/
    }
}
