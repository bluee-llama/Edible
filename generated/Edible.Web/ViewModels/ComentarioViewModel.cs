using System.ComponentModel.DataAnnotations;

namespace Edible.Web.ViewModels;

public sealed class ComentarioViewModel : IValidatableObject
{
    /*PROTECTED REGION ID(ComentarioViewModel_generated) START*/
    [Key]
    public int Id { get; set; }

    public int User_id { get; set; }

    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever]
    [Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    [System.ComponentModel.ReadOnly(true)]
    [UIHint("HariaImage")]
    public string? Imagen { get; set; }

    [DataType(DataType.Upload)]
    [Display(Name = "imagen")]
    [System.Text.Json.Serialization.JsonIgnore]
    public Microsoft.AspNetCore.Http.IFormFile? ImagenUpload { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    public string Titulo { get; set; } = string.Empty;

    [UIHint("HariaReadOnly:Create,Edit")]
    public string? Contenido { get; set; }

    public int Valoracion { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [UIHint("HariaAssociation")]
    [Display(Name = "Comentador")]
    public int? ComentadorId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> ComentadorOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "Comentador")]
    public string? ComentadorLabel { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [UIHint("HariaAssociation")]
    [Display(Name = "Receta")]
    public int? RecetaId { get; set; }
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, Microsoft.AspNetCore.Mvc.ModelBinding.Validation.ValidateNever]
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> RecetaOptions { get; set; } = new();
    [Microsoft.AspNetCore.Mvc.ModelBinding.BindNever, UIHint("HariaAssociationLabel"), Display(Name = "Receta")]
    public string? RecetaLabel { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        foreach (var result in ValidateAdditional(validationContext)) yield return result;
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ComentarioViewModel_manual) ENABLED START*/
    // Add custom properties and validation here.
    private IEnumerable<ValidationResult> ValidateAdditional(ValidationContext validationContext)
    {
        yield break;
    }

    /*PROTECTED REGION END*/
}
