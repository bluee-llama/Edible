using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

        /*PROTECTED REGION ID(usingEdible.Application.Services_IngredienteNevera_comprobarCaducidad) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace Edible.Application.Services;

public sealed partial class IngredienteNeveraService
{
    public async Task ComprobarCaducidadAsync(int p_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.Services_IngredienteNevera_comprobarCaducidad) ENABLED START*/
        // Write here your custom code...

        throw new ModelException("Application service generation for IngredienteNevera.comprobarCaducidad is pending.", new NotImplementedException("Method ComprobarCaducidadAsync() not yet implemented."));
        /*PROTECTED REGION END*/
    }
}
