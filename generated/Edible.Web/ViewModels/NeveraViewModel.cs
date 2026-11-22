using System.ComponentModel.DataAnnotations;

namespace Edible.Web.ViewModels;

public sealed class NeveraViewModel : IValidatableObject
{
    /*PROTECTED REGION ID(NeveraViewModel_generated) START*/
    [Key]
    public int Id { get; set; }

    [UIHint("HariaAssociation")]
    [Display(Name = "Usuario")]
    public int? UsuarioId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> UsuarioOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "Usuario")]
    public string? UsuarioLabel { get; set; }

    // HARIA-WEB COMPOSITION {"property":"IngredienteEnLaLista","fullName":"Edible.Web.ViewModels.IngredienteCompraViewModel","fields":[{"name":"Id","type":"int","key":true},{"name":"Cantidad","type":"int","key":false},{"name":"Comprado","type":"bool","key":false},{"name":"ListaCompraId","type":"int?","association":true},{"name":"ListaCompraLabel","type":"string?","readOnly":true},{"name":"IngredienteCatalogoId","type":"int?","association":true},{"name":"IngredienteCatalogoLabel","type":"string?","readOnly":true}]}
    [UIHint("HariaComposition")]
    [MinLength(0)]
    public List<IngredienteCompraViewModel> IngredienteEnLaLista { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public IngredienteCompraViewModel IngredienteEnLaListaTemplate { get; set; } = new();

    // HARIA-WEB COMPOSITION {"property":"IngredienteEnLaNevera","fullName":"Edible.Web.ViewModels.IngredienteNeveraViewModel","fields":[{"name":"Id","type":"int","key":true},{"name":"Cantidad","type":"int","key":false},{"name":"FechaCaducidad","type":"DateTime","key":false,"dataType":"Date"},{"name":"IngredienteCatalogoId","type":"int?","association":true},{"name":"IngredienteCatalogoLabel","type":"string?","readOnly":true},{"name":"RecetaId","type":"int?","association":true},{"name":"RecetaLabel","type":"string?","readOnly":true}]}
    [UIHint("HariaComposition")]
    [MinLength(0)]
    public List<IngredienteNeveraViewModel> IngredienteEnLaNevera { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public IngredienteNeveraViewModel IngredienteEnLaNeveraTemplate { get; set; } = new();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in ValidateAdditional(validationContext)) yield return result;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(NeveraViewModel_manual) ENABLED START*/
    // Add custom properties and validation here.
    private IEnumerable<ValidationResult> ValidateAdditional(ValidationContext validationContext)
    {
        yield break;
    }

    /*PROTECTED REGION END*/
}
