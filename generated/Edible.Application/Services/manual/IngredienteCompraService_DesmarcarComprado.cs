using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

        /*PROTECTED REGION ID(usingEdible.Application.Services_IngredienteCompra_desmarcarComprado) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace Edible.Application.Services;

public sealed partial class IngredienteCompraService
{
    public async Task DesmarcarCompradoAsync(int p_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.Services_IngredienteCompra_desmarcarComprado) ENABLED START*/
        // Write here your custom code...

        throw new ModelException("Application service generation for IngredienteCompra.desmarcarComprado is pending.", new NotImplementedException("Method DesmarcarCompradoAsync() not yet implemented."));
        /*PROTECTED REGION END*/
    }
}
