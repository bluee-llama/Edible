using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;

/*PROTECTED REGION ID(usingEdible.Application.Services_IngredienteCompra_desmarcarComprado) ENABLED START*/
// references to other libraries
/*PROTECTED REGION END*/

namespace Edible.Application.Services;

public sealed partial class IngredienteCompraService
{
    public async Task DesmarcarCompradoAsync(int p_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.Services_IngredienteCompra_desmarcarComprado) ENABLED START*/
        var entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("El ingrediente de compra no existe.");

        if (!entity.Comprado)
        {
            throw new ModelException("El ingrediente ya está desmarcado.");
        }

        entity.Comprado = false;

        await _repository.SaveAsync(entity);
        /*PROTECTED REGION END*/
    }
}