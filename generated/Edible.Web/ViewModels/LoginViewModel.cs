// HARIA-WEB GENERATED sha256:75aa6a61d398b4d1aa053b03af199d2d7904eef89ba37bc827daa066cb40ab81
using System.ComponentModel.DataAnnotations;

namespace Edible.Web.ViewModels;

public sealed class LoginViewModel
{
    public string UserClass { get; set; } = "Usuario";
    public string? Identifier { get; set; }
    [Display(Name = "id")]
    public int Id { get; set; }

    [Required(ErrorMessage = "Debes indicar {0}.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}
