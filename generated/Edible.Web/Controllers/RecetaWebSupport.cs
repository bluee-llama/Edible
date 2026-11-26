using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Rendering;
using Edible.Application.Abstractions;
using Edible.Application.Services;
using Edible.Domain.Entities;
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

/// <summary>Información nutricional total de una receta y alérgenos que contiene (REQ-06 y REQ-07).</summary>
public sealed record RecetaNutricion(
    int Calorias,
    float Proteinas,
    float Carbohidratos,
    float Grasas,
    IReadOnlyList<Alergenos> Alergenos,
    int IngredientesSinDatos);

/// <summary>
/// Apoyo de lectura para las vistas manuales de Receta (BuscarNombre, RecetasPorNevera,
/// Compartir, ComprobarIngredientesDisponibles y ficha de receta). No escribe datos:
/// solo carga opciones de desplegables, calcula datos para mostrar y decide qué puede ver o gestionar
/// el usuario autenticado.
/// </summary>
public static class RecetaWebSupport
{
    public const string RolAdministrador = "Administrador";

    /// <summary>Id del usuario autenticado (la cookie guarda el id en ClaimTypes.Name).</summary>
    public static int? CurrentUserId(ClaimsPrincipal user) =>
        int.TryParse(user.Identity?.Name, NumberStyles.Integer, CultureInfo.InvariantCulture, out var id) ? id : null;

    public static bool IsAdmin(ClaimsPrincipal user) => user.IsInRole(RolAdministrador);

    /// <summary>Un administrador puede usar cualquier chef/nevera/receta; el resto, solo los suyos.</summary>
    public static bool CanUseUser(ClaimsPrincipal user, int ownerId) =>
        IsAdmin(user) || (ownerId > 0 && CurrentUserId(user) == ownerId);

    /// <summary>Puede crear recetas quien sea Chef o Administrador.</summary>
    public static bool CanCreateReceta(ClaimsPrincipal user) =>
        user.IsInRole("Chef") || IsAdmin(user);

    /// <summary>Un Chef solo gestiona sus recetas; el administrador cualquiera (moderación).</summary>
    public static bool CanManageReceta(ClaimsPrincipal user, int? chefId) =>
        CanCreateReceta(user) && CanUseUser(user, chefId ?? 0);

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

    /// <summary>Id de la nevera del usuario autenticado, o null si no tiene.</summary>
    public static async Task<int?> FindMiNeveraIdAsync(IUnitOfWorkFactory factory, ClaimsPrincipal user)
    {
        var actual = CurrentUserId(user);
        if (actual is null) return null;
        await using var unitOfWork = factory.Create();
        var neveras = await unitOfWork.NeveraRepository.ReadAllAsync();
        return neveras.FirstOrDefault(nevera => nevera.Usuario is not null && nevera.Usuario.Id == actual)?.Id;
    }

    /// <summary>
    /// ¿Se puede cocinar la receta con la nevera del usuario autenticado?
    /// Usa la operación de negocio RecetasPorNevera. Devuelve null si el usuario no tiene nevera.
    /// </summary>
    public static async Task<bool?> PuedeCocinarseConMiNeveraAsync(IUnitOfWorkFactory factory, RecetaService recetaService, ClaimsPrincipal user, int recetaId)
    {
        var neveraId = await FindMiNeveraIdAsync(factory, user);
        if (neveraId is null) return null;
        var cocinables = await recetaService.RecetasPorNeveraAsync(neveraId.Value);
        return cocinables.Any(receta => receta.Id == recetaId);
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

    /// <summary>
    /// Calcula la información nutricional total de la receta (REQ-06) y reúne sus alérgenos (REQ-07).
    /// Convención: los valores nutricionales de cada Ingrediente son por 100 g/ml y
    /// LineaReceta.CantidadNecesaria se expresa en g/ml, así que cada línea aporta valor × cantidad / 100.
    /// </summary>
    public static RecetaNutricion CalcularNutricion(Receta receta)
    {
        double calorias = 0, proteinas = 0, carbohidratos = 0, grasas = 0;
        var sinDatos = 0;
        var alergenos = new SortedSet<Alergenos>();

        foreach (var linea in receta.Lineas)
        {
            var ingrediente = linea.IngredienteCatalogo;
            if (ingrediente is null) continue;

            foreach (var alergeno in ingrediente.Alergenos)
                if (alergeno != Alergenos.ninguno) alergenos.Add(alergeno);

            if (ingrediente.Calorias is null && ingrediente.G_proteinas is null &&
                ingrediente.G_carbohidratos is null && ingrediente.G_grasas is null)
            {
                sinDatos++;
                continue;
            }

            var factor = linea.CantidadNecesaria / 100.0;
            calorias += (ingrediente.Calorias ?? 0) * factor;
            proteinas += (ingrediente.G_proteinas ?? 0) * factor;
            carbohidratos += (ingrediente.G_carbohidratos ?? 0) * factor;
            grasas += (ingrediente.G_grasas ?? 0) * factor;
        }

        return new RecetaNutricion(
            (int)Math.Round(calorias),
            (float)Math.Round(proteinas, 1),
            (float)Math.Round(carbohidratos, 1),
            (float)Math.Round(grasas, 1),
            alergenos.ToList(),
            sinDatos);
    }
}