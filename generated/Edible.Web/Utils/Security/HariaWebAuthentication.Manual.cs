using System.Security.Claims;
using Edible.Domain.Entities;

namespace Edible.Web.Utils.Security;

/// <summary>
/// Parte manual de la autenticación (roles de confianza calculados en el servidor).
///
/// Roles resultantes:
///   - "Usuario"       -> todo el que inicia sesión (lo concede RolesByUserClass en appsettings.Haria.json;
///                        un Chef también es un Usuario porque Chef : Usuario).
///   - "Chef"          -> la entidad cargada en el servidor es una instancia de Chef (herencia).
///   - "Administrador" -> el Usuario reservado "admin" (el modelo no tiene clase Administrador).
///
/// Los roles NUNCA proceden del formulario ni de la sesión: solo de la entidad leída en BD.
/// </summary>
public static partial class HariaWebAuthentication
{
    public const string RoleUsuario = "Usuario";
    public const string RoleChef = "Chef";
    public const string RoleAdministrador = "Administrador";

    /// <summary>Nombre reservado del administrador. No se puede registrar ni asignar a otro usuario.</summary>
    public const string AdministratorName = "admin";

    public static bool IsReservedAdministratorName(string? name) =>
        string.Equals(name?.Trim(), AdministratorName, StringComparison.OrdinalIgnoreCase);

    static partial void AddTrustedClaims(List<Claim> claims, string domainClass, object identifier, object? serverEntity)
    {
        if (serverEntity is not Usuario usuario) return;

        // Herencia Chef : Usuario. EF devuelve la instancia derivada, así que basta el patrón de tipo.
        if (usuario is Chef) AddRole(claims, RoleChef);

        if (IsReservedAdministratorName(usuario.Name)) AddRole(claims, RoleAdministrador);
    }

    private static void AddRole(List<Claim> claims, string role)
    {
        if (!claims.Any(c => c.Type == ClaimTypes.Role && c.Value == role))
            claims.Add(new Claim(ClaimTypes.Role, role));
    }
}

/// <summary>Utilidades para consultar la identidad de la petición (la fuente de verdad es la cookie).</summary>
public static class HariaWebUserExtensions
{
    /// <summary>Id del Usuario autenticado (ClaimTypes.Name = id), o null si no hay sesión.</summary>
    public static int? GetUsuarioId(this ClaimsPrincipal user) =>
        int.TryParse(user.Identity?.Name, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture, out var id) ? id : null;

    public static bool IsAdministrador(this ClaimsPrincipal user) =>
        user.IsInRole(HariaWebAuthentication.RoleAdministrador);

    /// <summary>true si el usuario autenticado es el propio registro o es Administrador.</summary>
    public static bool CanManageUsuario(this ClaimsPrincipal user, int usuarioId) =>
        user.IsAdministrador() || user.GetUsuarioId() == usuarioId;
}
