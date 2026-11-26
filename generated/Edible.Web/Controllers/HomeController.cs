using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Edible.Web.Utils.Security;
using Edible.Web.ViewModels;

namespace Edible.Web.Controllers;

public sealed class HomeController : Controller
{
    /// <summary>
    /// Portada distinta según el rol:
    ///  - Administrador: la vista muestra el panel backoffice (_BusinessNavigation); no se carga nada.
    ///  - Usuario / Chef: dashboard con sus recetas cocinables, dieta, valoradas, compartidas y chefs.
    ///  - Anónimo: portada de bienvenida con acceso e inscripción.
    /// </summary>
    public async Task<IActionResult> Index()
    {
        if (User.Identity?.IsAuthenticated != true || User.IsAdministrador()) return View();

        var usuarioId = User.GetUsuarioId();
        if (usuarioId is null) return View();

        // Si la cuenta se borró teniendo la cookie viva, el modelo es null y la vista lo explica.
        var dashboard = await UsuarioWebSupport.BuildDashboardAsync(HttpContext.RequestServices, usuarioId.Value);
        return View(dashboard);
    }

    public IActionResult Privacy() => View();

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
