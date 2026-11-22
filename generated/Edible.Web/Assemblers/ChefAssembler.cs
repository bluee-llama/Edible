// HARIA-WEB MVC managed assembler
using Edible.Domain.Entities;

namespace Edible.Web.Assemblers;

public static class ChefAssembler
{
    /*PROTECTED REGION ID(ChefAssembler_method_ToViewModel) START*/
    public static Edible.Web.ViewModels.ChefViewModel ToViewModel(Chef entity) =>
        new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Publico = entity.Publico,
            Dieta = entity.Dieta,
            NeveraId = entity.Nevera?.Id,
            NeveraLabel = entity.Nevera is null ? null : Convert.ToString(entity.Nevera.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            ListaCompraId = entity.ListaCompra?.Id,
            ListaCompraLabel = entity.ListaCompra is null ? null : Convert.ToString(entity.ListaCompra.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ChefAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(Edible.Web.ViewModels.ChefViewModel model, Chef entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.Name = model.Name;
        entity.Publico = model.Publico;
        entity.Dieta = model.Dieta;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ChefAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
