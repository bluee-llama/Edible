using Microsoft.Extensions.DependencyInjection;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Web.ViewModels;

namespace Edible.Web.Controllers;

/// <summary>
/// Apoyo de solo lectura para la portada (Home) y los perfiles. No escribe datos ni contiene
/// reglas de negocio: lee mediante los repositorios del UnitOfWork y proyecta a ViewModels.
/// </summary>
public static class UsuarioWebSupport
{
    private const int MaxPorSeccion = 6;

    public static RecetaResumenViewModel ToResumen(Receta receta) => new()
    {
        Id = receta.Id,
        Nombre = receta.Nombre,
        ChefNombre = receta.Chef?.Name ?? "-",
        Dieta = receta.DietaEspecial,
        ValoracionMedia = receta.ValoracionMedia,
        TiempoRealizacion = receta.TiempoRealizacion,
        Dificultad = receta.Dificultad
    };

    /// <summary>
    /// Una receta encaja con la dieta del usuario si coincide, o si es vegana y el usuario sigue
    /// una dieta vegetariana o sin lactosa (una receta vegana cumple ambas).
    /// </summary>
    public static bool DietaCompatible(DietaEspecial receta, DietaEspecial usuario) =>
        receta == usuario ||
        (receta == DietaEspecial.vegana && (usuario == DietaEspecial.vegetariana || usuario == DietaEspecial.sinLactosa));

    /// <summary>Portada de Usuario/Chef. Devuelve null si el usuario ya no existe.</summary>
    public static async Task<HomeDashboardViewModel?> BuildDashboardAsync(IServiceProvider services, int usuarioId)
    {
        var factory = services.GetRequiredService<IUnitOfWorkFactory>();
        var compartidas = services.GetRequiredService<IRecetasCompartidasQuery>();

        await using var unitOfWork = factory.Create();
        var usuario = await unitOfWork.UsuarioRepository.ReadAsync(usuarioId);
        if (usuario is null) return null;

        var model = new HomeDashboardViewModel
        {
            UsuarioId = usuario.Id,
            Nombre = usuario.Name,
            Dieta = usuario.Dieta,
            EsChef = usuario is Chef,
            NeveraId = usuario.Nevera?.Id
        };

        var todas = await unitOfWork.RecetaRepository.ReadAllAsync();

        // REQ-05 / CU-06: recetas que se pueden cocinar con lo que hay en MI nevera.
        if (model.NeveraId is int neveraId)
        {
            try
            {
                var ids = (await unitOfWork.RecetaRepository.RecetasPorNeveraAsync(neveraId))
                    .Select(receta => receta.Id)
                    .ToHashSet();
                // Una receta sin líneas "se puede cocinar" con cualquier nevera: no aporta nada, se omite.
                model.Cocinables = todas
                    .Where(receta => ids.Contains(receta.Id) && receta.Lineas.Count > 0)
                    .OrderByDescending(receta => receta.ValoracionMedia)
                    .ThenBy(receta => receta.Nombre)
                    .Take(MaxPorSeccion)
                    .Select(ToResumen)
                    .ToList();
            }
            catch (Exception error) when (error is not OperationCanceledException)
            {
                // La portada no debe caerse porque falle un cálculo auxiliar.
                model.ErrorCocinables = true;
            }
        }

        // REQ-16: recomendadas según la dieta del usuario (si no tiene dieta especial no se filtra).
        if (usuario.Dieta != DietaEspecial.ninguna)
        {
            model.PorDieta = todas
                .Where(receta => DietaCompatible(receta.DietaEspecial, usuario.Dieta))
                .OrderByDescending(receta => receta.ValoracionMedia)
                .ThenBy(receta => receta.Nombre)
                .Take(MaxPorSeccion)
                .Select(ToResumen)
                .ToList();
        }

        // REQ-15: mejor valoradas y recientes (no hay fecha en el modelo: las más nuevas son las de mayor Id).
        model.MejorValoradas = todas
            .Where(receta => receta.ValoracionMedia > 0)
            .OrderByDescending(receta => receta.ValoracionMedia)
            .ThenBy(receta => receta.Nombre)
            .Take(MaxPorSeccion)
            .Select(ToResumen)
            .ToList();
        model.Recientes = todas
            .OrderByDescending(receta => receta.Id)
            .Take(MaxPorSeccion)
            .Select(ToResumen)
            .ToList();

        // Recetas que otros han compartido con este usuario (RecetasSugeridas).
        model.Compartidas = (await compartidas.ReadRecetasSugeridasAsync(usuario.Id))
            .OrderByDescending(receta => receta.Id)
            .Take(MaxPorSeccion)
            .Select(ToResumen)
            .ToList();

        // REQ-14: chefs con perfil público, los que más recetas tienen primero.
        var chefs = await unitOfWork.ChefRepository.ReadAllAsync();
        model.ChefsDestacados = chefs
            .Where(chef => chef.Publico)
            .Select(chef =>
            {
                var recetas = todas.Where(receta => receta.Chef?.Id == chef.Id).ToList();
                return new ChefDestacadoViewModel
                {
                    Id = chef.Id,
                    Nombre = chef.Name,
                    NumRecetas = recetas.Count,
                    NotaMedia = recetas.Where(receta => receta.ValoracionMedia > 0)
                        .Select(receta => receta.ValoracionMedia)
                        .DefaultIfEmpty(0f)
                        .Average()
                };
            })
            .OrderByDescending(chef => chef.NumRecetas)
            .ThenByDescending(chef => chef.NotaMedia)
            .ThenBy(chef => chef.Nombre)
            .Take(MaxPorSeccion)
            .ToList();

        return model;
    }

    /// <summary>
    /// Datos de un perfil. Con <paramref name="incluirPrivado"/> se añaden las recetas sugeridas y los
    /// comentarios del usuario (solo para el propio usuario o un Administrador). Null si no existe.
    /// </summary>
    public static async Task<PerfilExtrasViewModel?> BuildPerfilAsync(IServiceProvider services, int usuarioId, bool incluirPrivado)
    {
        var factory = services.GetRequiredService<IUnitOfWorkFactory>();
        var compartidas = services.GetRequiredService<IRecetasCompartidasQuery>();

        await using var unitOfWork = factory.Create();
        var usuario = await unitOfWork.UsuarioRepository.ReadAsync(usuarioId);
        if (usuario is null) return null;

        var model = new PerfilExtrasViewModel
        {
            UsuarioId = usuario.Id,
            Nombre = usuario.Name,
            Publico = usuario.Publico,
            Dieta = usuario.Dieta,
            EsChef = usuario is Chef,
            IncluyePrivado = incluirPrivado
        };

        if (model.EsChef)
        {
            var todas = await unitOfWork.RecetaRepository.ReadAllAsync();
            model.Publicadas = todas
                .Where(receta => receta.Chef?.Id == usuarioId)
                .OrderByDescending(receta => receta.Id)
                .Select(ToResumen)
                .ToList();
        }

        if (incluirPrivado)
        {
            model.Sugeridas = (await compartidas.ReadRecetasSugeridasAsync(usuarioId))
                .OrderByDescending(receta => receta.Id)
                .Select(ToResumen)
                .ToList();

            var comentarios = await unitOfWork.ComentarioRepository.ReadAllAsync();
            model.Comentarios = comentarios
                .Where(comentario => comentario.Comentador?.Id == usuarioId)
                .OrderByDescending(comentario => comentario.Id)
                .Select(comentario => new ComentarioResumenViewModel
                {
                    Id = comentario.Id,
                    RecetaId = comentario.Receta?.Id ?? 0,
                    RecetaNombre = comentario.Receta?.Nombre ?? "-",
                    Titulo = comentario.Titulo,
                    Valoracion = comentario.Valoracion,
                    Extracto = Extracto(comentario.Contenido),
                    TieneImagen = !string.IsNullOrWhiteSpace(comentario.Imagen)
                })
                .ToList();
        }

        return model;
    }

    private static string Extracto(string? contenido)
    {
        if (string.IsNullOrWhiteSpace(contenido)) return string.Empty;
        var texto = contenido.Trim();
        return texto.Length > 140 ? texto[..140] + "…" : texto;
    }
}
