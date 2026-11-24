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

    /*PROTECTED REGION ID(Edible.Application.Services_Receta_compartir_usuario) ENABLED START*/
    /// <summary>
    /// Comparte (sugiere) la receta con un usuario concreto y persiste la relación N:M
    /// <c>Receta.UsuariosSugeridos</c> / <c>Usuario.RecetasSugeridas</c>.
    /// El modelo TDM declara <c>compartir(p_oid)</c> sin destinatario, por eso esta sobrecarga
    /// recibe además el usuario con el que se comparte.
    /// </summary>
    public async Task CompartirAsync(int p_oid, int p_usuario_oid)
    {
        var receta = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("La receta no existe.");
        var usuario = await _unitOfWork.UsuarioRepository.ReadAsync(p_usuario_oid)
            ?? throw new ModelException("El usuario no existe.");

        if (receta.Chef is not null && receta.Chef.Id == usuario.Id)
            throw new ModelException("No se puede compartir una receta con su propio autor.");

        // Si la colección ya está cargada (misma sesión) evitamos el duplicado aquí.
        if (receta.UsuariosSugeridos.Any(u => u.Id == usuario.Id))
            throw new ModelException("La receta ya está compartida con ese usuario.");

        receta.UsuariosSugeridos.Add(usuario);
        try
        {
            await _repository.SaveAsync(receta);
        }
        catch (Exception ex)
        {
            // En una sesión nueva la colección no viene cargada: si el enlace ya existía,
            // la base de datos rechaza la fila duplicada de la tabla intermedia.
            receta.UsuariosSugeridos.Remove(usuario);
            throw new ModelException("No se ha podido compartir la receta (puede que ya estuviera compartida con ese usuario).", ex);
        }
    }
    /*PROTECTED REGION END*/
}