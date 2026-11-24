using System.ComponentModel.DataAnnotations;

namespace Edible.Web.ViewModels;

public sealed class IngredienteCompraViewModel : IValidatableObject
{
    /*PROTECTED REGION ID(IngredienteCompraViewModel_generated) ENABLED START*/
    [Key]
    public int Id { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; set; }

    public bool Comprado { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [UIHint("HariaAssociation")]
    [Display(Name = "Nevera")]
    public int? NeveraId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> NeveraOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "Nevera")]
    public string? NeveraLabel { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [UIHint("HariaAssociation")]
    [Display(Name = "ListaCompra")]
    public int? ListaCompraId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ListaCompraOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "ListaCompra")]
    public string? ListaCompraLabel { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [UIHint("HariaAssociation")]
    [Display(Name = "IngredienteCatalogo")]
    public int? IngredienteCatalogoId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> IngredienteCatalogoOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "IngredienteCatalogo")]
    public string? IngredienteCatalogoLabel { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in ValidateAdditional(validationContext)) yield return result;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(IngredienteCompraViewModel_manual) ENABLED START*/
    // Add custom properties and validation here.
    private IEnumerable<ValidationResult> ValidateAdditional(ValidationContext validationContext)
    {
        yield break;
    }

    /*PROTECTED REGION END*/
}
