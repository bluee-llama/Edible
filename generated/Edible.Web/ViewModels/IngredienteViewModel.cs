using System.ComponentModel.DataAnnotations;

namespace Edible.Web.ViewModels;

public sealed class IngredienteViewModel : IValidatableObject
{
    /*PROTECTED REGION ID(IngredienteViewModel_generated) START*/
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    public string Nombre { get; set; } = string.Empty;

    [EnumDataType(typeof(Edible.Domain.Enums.TipoIngrediente), ErrorMessage = "El valor de {0} no es válido.")]
    public Edible.Domain.Enums.TipoIngrediente Tipo { get; set; }

    public int? Calorias { get; set; }

    public float? G_proteinas { get; set; }

    public float? G_carbohidratos { get; set; }

    public float? G_grasas { get; set; }

    public float? G_racion { get; set; }

    public List<Edible.Domain.Enums.Alergenos> Alergenos { get; set; } = new();

    public List<Edible.Domain.Enums.DietaEspecial> DietaEspecial { get; set; } = new();



    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if ((Alergenos is null || Alergenos.Any(value => !Enum.IsDefined(typeof(Edible.Domain.Enums.Alergenos), value))))
            yield return new ValidationResult("El valor de Alergenos no es válido.", new[] { nameof(Alergenos) });
        if ((DietaEspecial is null || DietaEspecial.Any(value => !Enum.IsDefined(typeof(Edible.Domain.Enums.DietaEspecial), value))))
            yield return new ValidationResult("El valor de DietaEspecial no es válido.", new[] { nameof(DietaEspecial) });
        foreach (var result in ValidateAdditional(validationContext)) yield return result;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(IngredienteViewModel_manual) ENABLED START*/
    // Add custom properties and validation here.
    private IEnumerable<ValidationResult> ValidateAdditional(ValidationContext validationContext)
    {
        yield break;
    }

    /*PROTECTED REGION END*/
}
