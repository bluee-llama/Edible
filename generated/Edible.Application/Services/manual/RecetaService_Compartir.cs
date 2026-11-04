using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;

/*PROTECTED REGION ID(usingEdible.Application.Services_Receta_compartir) ENABLED START*/
// references to other libraries
/*PROTECTED REGION END*/

namespace Edible.Application.Services;

public sealed partial class RecetaService
{
    public async Task CompartirAsync(int p_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.Services_Receta_compartir) ENABLED START*/
        var entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("La receta no existe.");

        // Se persiste la lectura transaccional respetando los atributos del modelo TDM oficial.
        await _repository.SaveAsync(entity);
        /*PROTECTED REGION END*/
    }
}