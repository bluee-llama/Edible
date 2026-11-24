// Archivo parcial independiente (no lo regenera HARIA): acceso directo al perfil propio.
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Web.Utils.Security;

namespace Edible.Web.Controllers;

public partial class UsuarioController
{
    /// <summary>
    /// "Mi perfil": redirige al Details del usuario autenticado (funciona igual para Usuario y Chef,
    /// porque Chef : Usuario y comparten Id). La autorización de clase ya exige sesión.
    /// </summary>
    [HttpGet]
    public IActionResult Perfil()
    {
        var id = User.GetUsuarioId();
        if (id is null) return Challenge();
        return RedirectToAction(nameof(Details), new { id });
    }
}
