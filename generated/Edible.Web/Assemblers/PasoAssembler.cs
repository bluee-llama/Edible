// HARIA-WEB MVC managed assembler
using Edible.Domain.Entities;

namespace Edible.Web.Assemblers;

public static class PasoAssembler
{
    /*PROTECTED REGION ID(PasoAssembler_method_ToViewModel) START*/
    public static Edible.Web.ViewModels.PasoViewModel ToViewModel(Paso entity) =>
        new()
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            RecetaId = entity.Receta?.Id,
            RecetaLabel = entity.Receta is null ? null : Convert.ToString(entity.Receta.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(PasoAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(Edible.Web.ViewModels.PasoViewModel model, Paso entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.Nombre = model.Nombre;
        entity.Descripcion = model.Descripcion;
    }
    /*PROTECTED REGION END*/
    /*PROTECTED REGION ID(PasoAssembler_method_Create) START*/
    public static Paso Create(Edible.Web.ViewModels.PasoViewModel model)
    {
        var entity = new Paso();
        ApplyToEntity(model, entity);
        return entity;
    }
    /*PROTECTED REGION END*/
    /*PROTECTED REGION ID(PasoAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
