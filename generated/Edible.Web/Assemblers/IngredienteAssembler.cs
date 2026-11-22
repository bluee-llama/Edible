// HARIA-WEB MVC managed assembler
using Edible.Domain.Entities;

namespace Edible.Web.Assemblers;

public static class IngredienteAssembler
{
    /*PROTECTED REGION ID(IngredienteAssembler_method_ToViewModel) START*/
    public static Edible.Web.ViewModels.IngredienteViewModel ToViewModel(Ingrediente entity) =>
        new()
        {
            Id = entity.Id,
            Nombre = entity.Nombre,
            Tipo = entity.Tipo,
            Calorias = entity.Calorias,
            G_proteinas = entity.G_proteinas,
            G_carbohidratos = entity.G_carbohidratos,
            G_grasas = entity.G_grasas,
            G_racion = entity.G_racion,
            Alergenos = entity.Alergenos?.ToList() ?? new(),
            DietaEspecial = entity.DietaEspecial?.ToList() ?? new(),

        };
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(IngredienteAssembler_method_ApplyToEntity) START*/
    public static void ApplyToEntity(Edible.Web.ViewModels.IngredienteViewModel model, Ingrediente entity)
    {
        // Use validated input and a server-loaded entity. Persist through Application.
        // Identifiers, secrets and stored file paths are never copied from the client.
        ArgumentNullException.ThrowIfNull(model);
        ArgumentNullException.ThrowIfNull(entity);
        entity.Nombre = model.Nombre;
        entity.Tipo = model.Tipo;
        entity.Calorias = model.Calorias;
        entity.G_proteinas = model.G_proteinas;
        entity.G_carbohidratos = model.G_carbohidratos;
        entity.G_grasas = model.G_grasas;
        entity.G_racion = model.G_racion;
        entity.Alergenos = (model.Alergenos ?? new()).ToList();
        entity.DietaEspecial = (model.DietaEspecial ?? new()).ToList();
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(IngredienteAssembler_custom) ENABLED START*/
    // Add custom properties, validation helpers or actions here.
    /*PROTECTED REGION END*/
}
