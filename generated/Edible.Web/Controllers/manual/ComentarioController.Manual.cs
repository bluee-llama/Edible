// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(ComentarioController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Application.Exceptions;
using Edible.Web.Assemblers;
using Edible.Web.ViewModels;
using Edible.Application.UseCases.Comentario;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION ID(ComentarioController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// La autorización de clase ([Authorize(Roles = "Usuario,Chef,Administrador")]) está en ComentarioController.cs.
/*PROTECTED REGION END*/
public partial class ComentarioController
{
    // ------------------------------------------------------------------
    // Identidad y autoría (los roles los fija la configuración de seguridad)
    // ------------------------------------------------------------------

    private const string RolAdministrador = "Administrador";

    private bool IsAdministrador => User.IsInRole(RolAdministrador);

    // HariaWebAuthentication.SignInAsync guarda el Id del Usuario en ClaimTypes.Name.
    private int? CurrentUserId =>
        int.TryParse(User.Identity?.Name, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture, out var id) ? id : null;

    // Solo el autor o un administrador (moderación) pueden editar/borrar un comentario.
    private bool CanModify(Comentario entity) =>
        IsAdministrador || (entity.Comentador is not null && entity.Comentador.Id == CurrentUserId);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PublicarComentario(
        int recetaId,
        string titulo,
        string? contenido,
        int valoracion,
        IFormFile? imagenUpload)
    {
        if (CurrentUserId is not int currentUserId)
            return Challenge();
        if (recetaId <= 0)
            return BadRequest();
        if (string.IsNullOrWhiteSpace(titulo))
        {
            TempData["ComentarioError"] = "El título es obligatorio.";
            return RedirectToAction("Details", "Receta", new { id = recetaId });
        }
        if (valoracion < 1 || valoracion > 5)
        {
            TempData["ComentarioError"] = "La valoración debe estar entre 1 y 5.";
            return RedirectToAction("Details", "Receta", new { id = recetaId });
        }

        var cancellationToken = HttpContext?.RequestAborted ?? CancellationToken.None;
        if (imagenUpload is not null)
        {
            try { await _files.ValidateAsync(imagenUpload, cancellationToken); }
            catch (InvalidDataException error)
            {
                TempData["ComentarioError"] = error.Message;
                return RedirectToAction("Details", "Receta", new { id = recetaId });
            }
        }

        try
        {
            await using var unitOfWork = _useCaseUnitOfWorkFactory.Create();
            var useCase = new ComentarioUseCase(unitOfWork);
            var comentario = await useCase.RecalcularValoracionMediaAsync(
                1, titulo.Trim(), valoracion, currentUserId, recetaId);

            var contenidoPersistido = string.IsNullOrWhiteSpace(contenido) ? null : contenido.Trim();
            string? imagenPersistida = null;
            if (imagenUpload is not null)
                imagenPersistida = await _files.SaveAsync(imagenUpload, "Comentario", "Imagen", comentario.Id, cancellationToken);

            await using var saveUnitOfWork = _formUnitOfWorkFactory.Create();
            var saveService = new ComentarioService(saveUnitOfWork);
            await saveUnitOfWork.BeginTransactionAsync("MVC.Comentario.PublicarComentario");
            var comentarioGuardado = await saveService.ReadOIDComentarioDefaultAsync(comentario.Id);
            if (comentarioGuardado is null)
            {
                await saveUnitOfWork.RollbackAsync();
                if (imagenPersistida is not null) await _files.DeleteNewAsync(imagenPersistida);
                TempData["ComentarioError"] = "No se ha podido recuperar el comentario publicado.";
                return RedirectToAction("Details", "Receta", new { id = recetaId });
            }
            comentarioGuardado.Contenido = contenidoPersistido;
            comentarioGuardado.Imagen = imagenPersistida;
            await saveService.ModifierComentarioDefaultAsync(comentarioGuardado);
            await saveUnitOfWork.CommitAsync();

            TempData["ComentarioSuccess"] = "Comentario publicado correctamente.";
        }
        catch (ModelException error)
        {
            TempData["ComentarioError"] = error.InnerException?.Message ?? error.Message;
        }
        catch (Exception error)
        {
            _logger.LogError(error, "No se pudo publicar el comentario de la receta {RecetaId}.", recetaId);
            TempData["ComentarioError"] = "No se ha podido publicar el comentario.";
        }

        return RedirectToAction("Details", "Receta", new { id = recetaId });
    }

    // ------------------------------------------------------------------
    // RecalcularValoracionMedia (CustomTransaction)
    // ------------------------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> RecalcularValoracionMedia(int? recetaId = null)
    {
        var model = new RecalcularValoracionViewModel
        {
            RecetaId = recetaId,
            Valoracion = 5,
            ComentadorId = IsAdministrador ? null : CurrentUserId
        };
        await LoadRecalcularOptionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ComentarioController_RecalcularValoracionMedia_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> RecalcularValoracionMedia(int p_user_id, string p_titulo, int p_valoracion, int p_comentador_oid, int p_roleB_oid)
    {
    /*PROTECTED REGION ID(ComentarioController_RecalcularValoracionMedia_POST_body) ENABLED START*/

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Revisa los datos del formulario: título y valoración son obligatorios.";
            return RedirectToAction(nameof(RecalcularValoracionMedia), new { recetaId = p_roleB_oid });
        }

        // Un usuario no administrador solo puede publicar en su propio nombre.
        if (!IsAdministrador && CurrentUserId is int currentUser) p_comentador_oid = currentUser;

        try
        {
            await using var unitOfWork = _useCaseUnitOfWorkFactory.Create();
            var useCase = new Edible.Application.UseCases.Comentario.ComentarioUseCase(unitOfWork);
            var result = await useCase.RecalcularValoracionMediaAsync(p_user_id, p_titulo, p_valoracion, p_comentador_oid, p_roleB_oid);
            var media = result.Receta.ValoracionMedia;
            TempData["Success"] = string.Format(System.Globalization.CultureInfo.CurrentCulture,
                "Comentario publicado. La valoración media de «{0}» es ahora {1:0.0#}.", result.Receta.Nombre, media);
        }
        catch (ModelException error)
        {
            // El caso de uso envuelve la causa real (precondición incumplida, receta inexistente...).
            TempData["Error"] = error.InnerException?.Message ?? error.Message;
        }
        return RedirectToAction(nameof(RecalcularValoracionMedia), new { recetaId = p_roleB_oid });
    /*PROTECTED REGION END*/
    }

    private async Task LoadRecalcularOptionsAsync(RecalcularValoracionViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var usuarios = await unitOfWork.UsuarioRepository.ReadAllAsync();
        var recetas = await unitOfWork.RecetaRepository.ReadAllAsync();

        model.ComentadorOptions = usuarios
            .Where(u => IsAdministrador || u.Id == CurrentUserId)
            .OrderBy(u => u.Name)
            .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = u.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Text = u.Name
            }).ToList();
        model.RecetaOptions = recetas
            .OrderBy(r => r.Nombre)
            .Select(r => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = r.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Text = r.Nombre
            }).ToList();
        model.Recetas = recetas
            .OrderBy(r => r.Nombre)
            .Select(r => new RecalcularValoracionViewModel.RecetaResumen(r.Id, r.Nombre, r.ValoracionMedia))
            .ToList();
    }
}
