using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using System;

/*PROTECTED REGION ID(usingEdible.Application.Services_IngredienteNevera_comprobarCaducidad) ENABLED START*/
// references to other libraries
/*PROTECTED REGION END*/

namespace Edible.Application.Services;

public sealed partial class IngredienteNeveraService
{
    public async Task ComprobarCaducidadAsync(int p_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.Services_IngredienteNevera_comprobarCaducidad) ENABLED START*/
        var entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("El ingrediente en nevera no existe.");

        // Se persiste la lectura transaccional respetando los atributos del modelo TDM oficial.
        await _repository.SaveAsync(entity);
        /*PROTECTED REGION END*/
    }
}