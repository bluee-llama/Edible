// HARIA-WEB MVC managed assembler
using Edible.Domain.Entities;

namespace Edible.Web.Assemblers;

public static class IngredienteCompraAssembler
{
    /*PROTECTED REGION ID(IngredienteCompraAssembler_method_ToViewModel) START*/
    public static Edible.Web.ViewModels.IngredienteCompraViewModel ToViewModel(IngredienteCompra entity) =>
        new()
        {
            Id = entity.Id,
            Cantidad = entity.Cantidad,
            Comprado = entity.Comprado,
            NeveraId = entity.Nevera?.Id,
            NeveraLabel = entity.Nevera is null ? null : Convert.ToString(entity.Nevera.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            ListaCompraId = entity.ListaCompra?.Id,
            ListaCompraLabel = entity.ListaCompra is null ? null : Convert.ToString(entity.ListaCompra.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            IngredienteCatalogoId = entity.IngredienteCatalogo?.Id,
            IngredienteCatalogoLabel = entity.IngredienteCatalogo is null ? null : Convert.ToString(entity.IngredienteCatalogo.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(IngredienteCompraAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(Edible.Web.ViewModels.IngredienteCompraViewModel model, IngredienteCompra entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.Cantidad = model.Cantidad;
        entity.Comprado = model.Comprado;
    }
    /*PROTECTED REGION END*/
    /*PROTECTED REGION ID(IngredienteCompraAssembler_method_Create) START*/
    public static IngredienteCompra Create(Edible.Web.ViewModels.IngredienteCompraViewModel model)
    {
        var entity = new IngredienteCompra();
        ApplyToEntity(model, entity);
        return entity;
    }
    /*PROTECTED REGION END*/
    /*PROTECTED REGION ID(IngredienteCompraAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
