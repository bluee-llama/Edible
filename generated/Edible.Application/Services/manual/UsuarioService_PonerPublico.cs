using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;

/*PROTECTED REGION ID(usingEdible.Application.Services_Usuario_ponerPublico) ENABLED START*/
// references to other libraries
/*PROTECTED REGION END*/

namespace Edible.Application.Services;

public sealed partial class UsuarioService
{
    public async Task PonerPublicoAsync(int p_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.Services_Usuario_ponerPublico) ENABLED START*/
        var entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("El usuario no existe.");

        if (entity.Publico == true)
        {
            throw new ModelException("El usuario ya es público.");
        }

        entity.Publico = true;

        await _repository.SaveAsync(entity);
        /*PROTECTED REGION END*/
    }
}