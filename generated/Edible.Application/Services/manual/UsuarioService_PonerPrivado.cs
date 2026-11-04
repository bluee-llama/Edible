using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
<<<<<<< HEAD

/*PROTECTED REGION ID(usingEdible.Application.Services_Usuario_ponerPrivado) ENABLED START*/
// references to other libraries
/*PROTECTED REGION END*/
=======
using Edible.Domain.Enums;

        // references to other libraries
        
>>>>>>> bbdcbb95ef26f24cd06e4610fa13c9b01fb6d5d4

namespace Edible.Application.Services;

public sealed partial class UsuarioService
{
<<<<<<< HEAD
    public async Task PonerPrivadoAsync(int p_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.Services_Usuario_ponerPrivado) ENABLED START*/
        var entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("El usuario no existe.");

        // Se persiste la lectura transaccional respetando los atributos del modelo TDM oficial.
        await _repository.SaveAsync(entity);
        /*PROTECTED REGION END*/
    }
}
=======
    /*PROTECTED REGION ID(usingEdible.Application.Services_Usuario_ponerPrivado) ENABLED START*/

    public async Task PonerPrivadoAsync(int p_oid)
    {
        var usuario = await _repository.ReadAsync(p_oid) 
            ?? throw new ModelException("El usuario especificado no existe.");

        if (!usuario.EsPublico)
        {
            throw new ModelException("El perfil del usuario ya es privado.");
        }

        usuario.EsPublico = false;
        await _repository.SaveAsync(usuario);
    }
    /*PROTECTED REGION END*/
}
>>>>>>> bbdcbb95ef26f24cd06e4610fa13c9b01fb6d5d4
