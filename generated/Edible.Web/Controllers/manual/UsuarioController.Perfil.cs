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

    /// <summary>
    /// Perfil público de un usuario o chef (REQ-14): nombre, dieta y recetas publicadas.
    /// Solo se puede abrir si el perfil es público, o si es el propio usuario o un Administrador.
    /// No expone datos privados (recetas guardadas ni comentarios).
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> PerfilPublico(int id)
    {
        var perfil = await UsuarioWebSupport.BuildPerfilAsync(HttpContext.RequestServices, id, incluirPrivado: false);
        if (perfil is null) return NotFound();
        if (!perfil.Publico && !User.CanManageUsuario(id)) return Forbid();
        return View(perfil);
    }
}
