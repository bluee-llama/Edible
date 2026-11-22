// HARIA-WEB MVC managed assembler
using Edible.Domain.Entities;

namespace Edible.Web.Assemblers;

public static class LineaRecetaAssembler
{
    /*PROTECTED REGION ID(LineaRecetaAssembler_method_ToViewModel) START*/
    public static Edible.Web.ViewModels.LineaRecetaViewModel ToViewModel(LineaReceta entity) =>
        new()
        {
            Id = entity.Id,
            CantidadNecesaria = entity.CantidadNecesaria,
            RecetaId = entity.Receta?.Id,
            RecetaLabel = entity.Receta is null ? null : Convert.ToString(entity.Receta.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            IngredienteCatalogoId = entity.IngredienteCatalogo?.Id,
            IngredienteCatalogoLabel = entity.IngredienteCatalogo is null ? null : Convert.ToString(entity.IngredienteCatalogo.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(LineaRecetaAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(Edible.Web.ViewModels.LineaRecetaViewModel model, LineaReceta entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.CantidadNecesaria = model.CantidadNecesaria;
    }
    /*PROTECTED REGION END*/
    /*PROTECTED REGION ID(LineaRecetaAssembler_method_Create) START*/
    public static LineaReceta Create(Edible.Web.ViewModels.LineaRecetaViewModel model)
    {
        var entity = new LineaReceta();
        ApplyToEntity(model, entity);
        return entity;
    }
    /*PROTECTED REGION END*/
    /*PROTECTED REGION ID(LineaRecetaAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
