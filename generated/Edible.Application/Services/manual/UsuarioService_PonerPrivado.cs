using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;

/*PROTECTED REGION ID(usingEdible.Application.Services_Usuario_ponerPrivado) ENABLED START*/
// references to other libraries
/*PROTECTED REGION END*/

namespace Edible.Application.Services;

public sealed partial class UsuarioService
{
    public async Task PonerPrivadoAsync(int p_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.Services_Usuario_ponerPrivado) ENABLED START*/
        var entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("El usuario no existe.");

        if (entity.Publico == false)
        {
            throw new ModelException("El usuario ya es privado.");
        }

        entity.Publico = false;

        await _repository.SaveAsync(entity);
        /*PROTECTED REGION END*/
    }
}