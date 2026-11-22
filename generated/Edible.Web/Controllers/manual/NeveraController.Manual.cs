// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(NeveraController_manual_usings) ENABLED START*/
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
/*PROTECTED REGION ID(NeveraController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION END*/
public partial class NeveraController
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(NeveraController_RecorrerListaCompra_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> RecorrerListaCompra(int p_usuario_oid)
    {
    /*PROTECTED REGION ID(NeveraController_RecorrerListaCompra_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return BadRequest(ModelState);
        await using var unitOfWork = _useCaseUnitOfWorkFactory.Create();
        var useCase = new Edible.Application.UseCases.Nevera.NeveraUseCase(unitOfWork);
        var result = await useCase.RecorrerListaCompraAsync(p_usuario_oid);
        return Ok(result);
    /*PROTECTED REGION END*/
    }
}
