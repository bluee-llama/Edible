// HARIA-WEB MVC managed assembler
using Edible.Domain.Entities;

namespace Edible.Web.Assemblers;

public static class IngredienteNeveraAssembler
{
    /*PROTECTED REGION ID(IngredienteNeveraAssembler_method_ToViewModel) START*/
    public static Edible.Web.ViewModels.IngredienteNeveraViewModel ToViewModel(IngredienteNevera entity) =>
        new()
        {
            Id = entity.Id,
            Cantidad = entity.Cantidad,
            FechaCaducidad = entity.FechaCaducidad,
            IngredienteCatalogoId = entity.IngredienteCatalogo?.Id,
            IngredienteCatalogoLabel = entity.IngredienteCatalogo is null ? null : Convert.ToString(entity.IngredienteCatalogo.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            NeveraId = entity.Nevera?.Id,
            NeveraLabel = entity.Nevera is null ? null : Convert.ToString(entity.Nevera.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            RecetaId = entity.Receta?.Id,
            RecetaLabel = entity.Receta is null ? null : Convert.ToString(entity.Receta.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(IngredienteNeveraAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(Edible.Web.ViewModels.IngredienteNeveraViewModel model, IngredienteNevera entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.Cantidad = model.Cantidad;
        entity.FechaCaducidad = model.FechaCaducidad;
    }
    /*PROTECTED REGION END*/
    /*PROTECTED REGION ID(IngredienteNeveraAssembler_method_Create) START*/
    public static IngredienteNevera Create(Edible.Web.ViewModels.IngredienteNeveraViewModel model)
    {
        var entity = new IngredienteNevera();
        ApplyToEntity(model, entity);
        return entity;
    }
    /*PROTECTED REGION END*/
    /*PROTECTED REGION ID(IngredienteNeveraAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
