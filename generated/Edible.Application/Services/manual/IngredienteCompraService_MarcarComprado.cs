using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;

/*PROTECTED REGION ID(usingEdible.Application.Services_IngredienteCompra_marcarComprado) ENABLED START*/
// references to other libraries
/*PROTECTED REGION END*/

namespace Edible.Application.Services;

public sealed partial class IngredienteCompraService
{
    public async Task MarcarCompradoAsync(int p_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.Services_IngredienteCompra_marcarComprado) ENABLED START*/
        var entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("El ingrediente de compra no existe.");

        if (entity.Comprado)
        {
            throw new ModelException("El ingrediente ya está marcado como comprado.");
        }

        entity.Comprado = true;

        await _repository.SaveAsync(entity);
        /*PROTECTED REGION END*/
    }
}