// HARIA-WEB MVC managed assembler
using Edible.Domain.Entities;

namespace Edible.Web.Assemblers;

public static class RecetaAssembler
{
    /*PROTECTED REGION ID(RecetaAssembler_method_ToViewModel) START*/
    public static Edible.Web.ViewModels.RecetaViewModel ToViewModel(Receta entity) =>
        new()
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Descripcion = entity.Descripcion,
            DietaEspecial = entity.DietaEspecial,
            ValoracionMedia = entity.ValoracionMedia,
            TiempoRealizacion = entity.TiempoRealizacion,
            Dificultad = entity.Dificultad,
            ChefId = entity.Chef?.Id,
            ChefLabel = entity.Chef is null ? null : Convert.ToString(entity.Chef.Name, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            Pasos = entity.Pasos.Select(PasoAssembler.ToViewModel).ToList(),
            Lineas = entity.Lineas.Select(LineaRecetaAssembler.ToViewModel).ToList(),
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(RecetaAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(Edible.Web.ViewModels.RecetaViewModel model, Receta entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.Nombre = model.Nombre;
        entity.Descripcion = model.Descripcion;
        entity.DietaEspecial = model.DietaEspecial;
        entity.ValoracionMedia = model.ValoracionMedia;
        entity.TiempoRealizacion = model.TiempoRealizacion;
        entity.Dificultad = model.Dificultad;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(RecetaAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
