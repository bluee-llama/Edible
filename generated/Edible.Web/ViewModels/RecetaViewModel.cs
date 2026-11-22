using System.ComponentModel.DataAnnotations;

namespace Edible.Web.ViewModels;

public sealed class RecetaViewModel : IValidatableObject
{
    /*PROTECTED REGION ID(RecetaViewModel_generated) START*/
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debes indicar {0}.")]
    public string Descripcion { get; set; } = string.Empty;

    [EnumDataType(typeof(Edible.Domain.Enums.DietaEspecial), ErrorMessage = "El valor de {0} no es válido.")]
    public Edible.Domain.Enums.DietaEspecial DietaEspecial { get; set; }

    public float ValoracionMedia { get; set; }

    public int TiempoRealizacion { get; set; }

    public int Dificultad { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [UIHint("HariaAssociation")]
    [Display(Name = "Chef")]
    public int? ChefId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ChefOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "Chef")]
    public string? ChefLabel { get; set; }

    // HARIA-WEB COMPOSITION {"property":"Pasos","fullName":"Edible.Web.ViewModels.PasoViewModel","fields":[{"name":"Id","type":"int","key":true},{"name":"Nombre","type":"string","key":false},{"name":"Descripcion","type":"string","key":false}]}
    [UIHint("HariaComposition")]
    [MinLength(0)]
    public List<PasoViewModel> Pasos { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public PasoViewModel PasosTemplate { get; set; } = new();

    // HARIA-WEB COMPOSITION {"property":"Lineas","fullName":"Edible.Web.ViewModels.LineaRecetaViewModel","fields":[{"name":"Id","type":"int","key":true},{"name":"CantidadNecesaria","type":"int","key":false},{"name":"IngredienteCatalogoId","type":"int?","association":true},{"name":"IngredienteCatalogoLabel","type":"string?","readOnly":true}]}
    [UIHint("HariaComposition")]
    [MinLength(1)]
    public List<LineaRecetaViewModel> Lineas { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public LineaRecetaViewModel LineasTemplate { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in ValidateAdditional(validationContext)) yield return result;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(RecetaViewModel_manual) ENABLED START*/
    // Add custom properties and validation here.
    private IEnumerable<ValidationResult> ValidateAdditional(ValidationContext validationContext)
    {
        yield break;
    }

    /*PROTECTED REGION END*/
}
