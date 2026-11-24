using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Edible.Application.Abstractions;
using Edible.Domain.Enums;

namespace Edible.Web.Controllers;

/// <summary>Fila del desplegable "Comprobar ingredientes": una receta y el chef (dueño de la nevera) que la prepara.</summary>
public sealed record RecetaCheckOption(
    string Nombre,
    string Descripcion,
    DietaEspecial DietaEspecial,
    float ValoracionMedia,
    int ChefId,
    string ChefNombre);

/// <summary>
/// Apoyo de lectura para las vistas manuales de Receta (BuscarNombre, RecetasPorNevera,
/// Compartir y ComprobarIngredientesDisponibles). No contiene lógica de negocio ni escribe datos:
/// solo carga opciones de desplegables y decide qué datos puede ver el usuario autenticado.
/// </summary>
public static class RecetaWebSupport
{
    public const string RolAdministrador = "Administrador";

    /// <summary>Id del usuario autenticado (la cookie guarda el id en ClaimTypes.Name).</summary>
    public static int? CurrentUserId(ClaimsPrincipal user) =>
        int.TryParse(user.Identity?.Name, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) ? id : null;

    public static bool IsAdmin(ClaimsPrincipal user) => user.IsInRole(RolAdministrador);

    /// <summary>Un administrador puede usar cualquier chef/nevera; el resto, solo los suyos.</summary>
    public static bool CanUseUser(ClaimsPrincipal user, int ownerId) =>
        IsAdmin(user) || CurrentUserId(user) == ownerId;

    /// <summary>Neveras que el usuario puede consultar: la suya (todas si es administrador).</summary>
    public static async Task<List<SelectListItem>> LoadNeveraOptionsAsync(IUnitOfWorkFactory factory, ClaimsPrincipal user)
    {
        await using var unitOfWork = factory.Create();
        var neveras = await unitOfWork.NeveraRepository.ReadAllAsync();
        return neveras
            .Where(nevera => nevera.Usuario is not null && CanUseUser(user, nevera.Usuario.Id))
            .OrderBy(nevera => nevera.Usuario!.Name)
            .Select(nevera => new SelectListItem
            {
                Value = nevera.Id.ToString(CultureInfo.InvariantCulture),
                Text = $"Nevera de {nevera.Usuario!.Name}"
            })
            .ToList();
    }

    /// <summary>Posibles destinatarios al compartir una receta: todos menos el usuario actual y el autor.</summary>
    public static async Task<List<SelectListItem>> LoadUsuarioOptionsAsync(IUnitOfWorkFactory factory, ClaimsPrincipal user, int? autorId)
    {
        await using var unitOfWork = factory.Create();
        var actual = CurrentUserId(user);
        var usuarios = await unitOfWork.UsuarioRepository.ReadAllAsync();
        return usuarios
            .Where(usuario => usuario.Id != actual && usuario.Id != autorId)
            .OrderBy(usuario => usuario.Name)
            .Select(usuario => new SelectListItem
            {
                Value = usuario.Id.ToString(CultureInfo.InvariantCulture),
                Text = usuario.Name
            })
            .ToList();
    }

    /// <summary>Recetas que el usuario puede comprobar contra la nevera de su chef (las suyas; todas si es administrador).</summary>
    public static async Task<List<RecetaCheckOption>> LoadRecetaChecksAsync(IUnitOfWorkFactory factory, ClaimsPrincipal user)
    {
        await using var unitOfWork = factory.Create();
        var recetas = await unitOfWork.RecetaRepository.ReadAllAsync();
        return recetas
            .Where(receta => receta.Chef is not null && CanUseUser(user, receta.Chef.Id))
            .OrderBy(receta => receta.Nombre)
            .Select(receta => new RecetaCheckOption(
                receta.Nombre,
                string.IsNullOrWhiteSpace(receta.Descripcion) ? "-" : receta.Descripcion,
                receta.DietaEspecial,
                receta.ValoracionMedia,
                receta.Chef.Id,
                receta.Chef.Name))
            .ToList();
    }
}