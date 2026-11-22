// HARIA-WEB MVC managed assembler
using Edible.Domain.Entities;

namespace Edible.Web.Assemblers;

public static class NeveraAssembler
{
    /*PROTECTED REGION ID(NeveraAssembler_method_ToViewModel) START*/
    public static Edible.Web.ViewModels.NeveraViewModel ToViewModel(Nevera entity) =>
        new()
        {
            Id = entity.Id,
            UsuarioId = entity.Usuario?.Id,
            UsuarioLabel = entity.Usuario is null ? null : Convert.ToString(entity.Usuario.Name, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
            IngredienteEnLaLista = entity.IngredienteEnLaLista.Select(IngredienteCompraAssembler.ToViewModel).ToList(),
            IngredienteEnLaNevera = entity.IngredienteEnLaNevera.Select(IngredienteNeveraAssembler.ToViewModel).ToList(),
        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(NeveraAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(Edible.Web.ViewModels.NeveraViewModel model, Nevera entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);

    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(NeveraAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
