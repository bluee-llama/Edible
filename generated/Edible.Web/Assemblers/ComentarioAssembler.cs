// HARIA-WEB MVC managed assembler
using Edible.Domain.Entities;

namespace Edible.Web.Assemblers;

public static class ComentarioAssembler
{
    /*PROTECTED REGION ID(ComentarioAssembler_method_ToViewModel) START*/
    public static Edible.Web.ViewModels.ComentarioViewModel ToViewModel(Comentario entity) =>
        new()
        {
            Id = entity.Id,
            User_id = entity.User_id,
            Imagen = entity.Imagen,
            Titulo = entity.Titulo,
            Contenido = entity.Contenido,
            Valoracion = entity.Valoracion,
            ComentadorId = entity.Comentador?.Id,
            ComentadorLabel = entity.Comentador is null ? null : Convert.ToString(entity.Comentador.Name, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            RecetaId = entity.Receta?.Id,
            RecetaLabel = entity.Receta is null ? null : Convert.ToString(entity.Receta.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ComentarioAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(Edible.Web.ViewModels.ComentarioViewModel model, Comentario entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.User_id = model.User_id;
        entity.Titulo = model.Titulo;
        entity.Contenido = model.Contenido;
        entity.Valoracion = model.Valoracion;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ComentarioAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
