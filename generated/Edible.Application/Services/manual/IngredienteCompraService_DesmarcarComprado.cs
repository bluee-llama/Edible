using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
<<<<<<< HEAD

/*PROTECTED REGION ID(usingEdible.Application.Services_IngredienteCompra_desmarcarComprado) ENABLED START*/
// references to other libraries
/*PROTECTED REGION END*/
=======
using Edible.Domain.Enums;

        // references to other libraries
>>>>>>> bbdcbb95ef26f24cd06e4610fa13c9b01fb6d5d4

namespace Edible.Application.Services;

public sealed partial class IngredienteCompraService
{
<<<<<<< HEAD
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
=======
    /*PROTECTED REGION ID(usingEdible.Application.Services_IngredienteCompra_desmarcarComprado) ENABLED START*/

    public async Task DesmarcarCompradoAsync(int p_oid)
    {
        var ingrediente = await _repository.ReadAsync(p_oid) 
            ?? throw new ModelException("El ingrediente de compra especificado no existe.");

        if (!ingrediente.Comprado)
        {
            throw new ModelException("El ingrediente ya se encuentra desmarcado.");
        }

        ingrediente.Comprado = false;
        await _repository.SaveAsync(ingrediente);
    }
    /*PROTECTED REGION END*/

}
>>>>>>> bbdcbb95ef26f24cd06e4610fa13c9b01fb6d5d4
