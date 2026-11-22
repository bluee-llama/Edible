// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(ComentarioController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

/*PROTECTED REGION ID(ComentarioController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class ComentarioController : Controller
{
    /*PROTECTED REGION ID(ComentarioController_dependencies) START*/
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _useCaseUnitOfWorkFactory;
    private readonly ComentarioService _comentarioService;
    private readonly ILogger<ComentarioController> _logger;
    private readonly Edible.Web.Utils.Files.IWebFileStorage _files;
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _fileUnitOfWorkFactory;
    private readonly ILogger<ComentarioController> _fileLogger;
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public ComentarioController(ComentarioService comentarioService, Edible.Web.Utils.Files.IWebFileStorage files, Edible.Application.Abstractions.IUnitOfWorkFactory fileUnitOfWorkFactory, ILogger<ComentarioController> fileLogger, Edible.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, Edible.Application.Abstractions.IUnitOfWorkFactory useCaseUnitOfWorkFactory)
    {
        _comentarioService = comentarioService;
        _files = files;
        _fileUnitOfWorkFactory = fileUnitOfWorkFactory;
        _fileLogger = fileLogger;
        _formUnitOfWorkFactory = formUnitOfWorkFactory;
        _useCaseUnitOfWorkFactory = useCaseUnitOfWorkFactory;
        _logger = fileLogger;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(ComentarioController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(ComentarioController_Index_GET_body) START*/

        var entities = await _comentarioService.ReadAllAsync();
        return View(entities.Select(ComentarioAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ComentarioController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(ComentarioController_Details_GET_body) START*/

        var entity = await _comentarioService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(ComentarioAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ComentarioController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(ComentarioController_Create_GET_body) START*/

        var model = new Edible.Web.ViewModels.ComentarioViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ComentarioController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(Edible.Web.ViewModels.ComentarioViewModel model)
    {
    /*PROTECTED REGION ID(ComentarioController_Create_POST_body) START*/

        ModelState.Remove(nameof(model.Contenido)); // Campo fuera del contrato Create de Application.
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.ComentarioService(unitOfWork);


        model.Imagen = null;


        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ComentadorId is int selectedComentador &&
            !model.ComentadorOptions.Any(option => option.Value == Convert.ToString(selectedComentador, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ComentadorId", "Selecciona un registro existente de Usuario.");
        if (model.ComentadorId is null) ModelState.AddModelError("ComentadorId", "Debes seleccionar una opción.");
        if (model.RecetaId is int selectedReceta &&
            !model.RecetaOptions.Any(option => option.Value == Convert.ToString(selectedReceta, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("RecetaId", "Selecciona un registro existente de Receta.");
        if (model.RecetaId is null) ModelState.AddModelError("RecetaId", "Debes seleccionar una opción.");



        var cancellationToken = HttpContext?.RequestAborted ?? CancellationToken.None;
        await ValidateUploadsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var newFiles = new List<string>();
        var commitStarted = false;

        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Comentario.Create");


            var entity = await service.NewAsync(model.User_id, model.Titulo, model.Valoracion, model.ComentadorId.GetValueOrDefault(), model.RecetaId.GetValueOrDefault());

            await service.ApplyComentarioRelationsAsync(entity, p_comentador_id: model.ComentadorId, p_receta_id: model.RecetaId);
            if (true && model.ImagenUpload is not null)
            {
                var stored = await _files.SaveAsync(model.ImagenUpload, "Comentario", "Imagen", entity.Id, cancellationToken);
                newFiles.Add(stored);
                entity.Imagen = stored;
            }
            await service.ModifierComentarioDefaultAsync(entity);

            commitStarted = true;
            await unitOfWork.CommitAsync();
        }
        catch (Exception error)
        {
            // Retain uploads if the commit outcome is uncertain.
            var rolledBack = !commitStarted;
            try { await unitOfWork.RollbackAsync(); }
            catch (Exception rollbackError) { rolledBack = false; _logger.LogError(rollbackError, "Form rollback failed; uploads retained for reconciliation."); }
            if (commitStarted) _logger.LogWarning(error, "Form commit outcome is uncertain; uploads retained for reconciliation.");
            if (rolledBack) foreach (var stored in newFiles) await _files.DeleteNewAsync(stored);
            if (error is OperationCanceledException) throw;
            _logger.LogError(error, "Could not save the record and its lines.");
            ModelState.AddModelError(string.Empty, "No se han podido guardar el registro y sus líneas.");
            return View(model);
        }


        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ComentarioController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(ComentarioController_Edit_GET_body) START*/

        var entity = await _comentarioService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = ComentarioAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ComentarioController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, Edible.Web.ViewModels.ComentarioViewModel model)
    {
    /*PROTECTED REGION ID(ComentarioController_Edit_POST_body) START*/

        ModelState.Remove(nameof(model.Contenido)); // Campo fuera del contrato Edit de Application.
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.ComentarioService(unitOfWork);

        var entity = await service.ReadOIDComentarioDefaultAsync(id);
        if (entity is null) return NotFound();
        var previousImagen = entity.Imagen;
        model.Imagen = previousImagen;


        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ComentadorId is int selectedComentador &&
            !model.ComentadorOptions.Any(option => option.Value == Convert.ToString(selectedComentador, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ComentadorId", "Selecciona un registro existente de Usuario.");
        if (model.ComentadorId is null) ModelState.AddModelError("ComentadorId", "Debes seleccionar una opción.");
        if (model.RecetaId is int selectedReceta &&
            !model.RecetaOptions.Any(option => option.Value == Convert.ToString(selectedReceta, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("RecetaId", "Selecciona un registro existente de Receta.");
        if (model.RecetaId is null) ModelState.AddModelError("RecetaId", "Debes seleccionar una opción.");



        var cancellationToken = HttpContext?.RequestAborted ?? CancellationToken.None;
        await ValidateUploadsAsync(model, cancellationToken);
        if (!ModelState.IsValid) return View(model);
        var newFiles = new List<string>();
        var commitStarted = false;

        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Comentario.Edit");


            await service.ModifyAsync(id, model.User_id, model.Titulo, model.Valoracion);
            entity = await service.ReadOIDComentarioDefaultAsync(id) ?? throw new InvalidOperationException("Entity disappeared during update.");
            await service.ApplyComentarioRelationsAsync(entity, p_comentador_id: model.ComentadorId, p_receta_id: model.RecetaId);
            if (true && model.ImagenUpload is not null)
            {
                var stored = await _files.SaveAsync(model.ImagenUpload, "Comentario", "Imagen", entity.Id, cancellationToken);
                newFiles.Add(stored);
                entity.Imagen = stored;
            }
            await service.ModifierComentarioDefaultAsync(entity);

            commitStarted = true;
            await unitOfWork.CommitAsync();
        }
        catch (Exception error)
        {
            // Retain uploads if the commit outcome is uncertain.
            var rolledBack = !commitStarted;
            try { await unitOfWork.RollbackAsync(); }
            catch (Exception rollbackError) { rolledBack = false; _logger.LogError(rollbackError, "Form rollback failed; uploads retained for reconciliation."); }
            if (commitStarted) _logger.LogWarning(error, "Form commit outcome is uncertain; uploads retained for reconciliation.");
            if (rolledBack) foreach (var stored in newFiles) await _files.DeleteNewAsync(stored);
            if (error is OperationCanceledException) throw;
            _logger.LogError(error, "Could not save the record and its lines.");
            ModelState.AddModelError(string.Empty, "No se han podido guardar el registro y sus líneas.");
            return View(model);
        }
        if (true && model.ImagenUpload is not null) await _files.DeleteReplacedAsync(previousImagen);

        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ComentarioController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(ComentarioController_Delete_GET_body) START*/

        var entity = await _comentarioService.ReadOIDComentarioDefaultAsync(id);
        if (entity is null) return NotFound();
        return View(ComentarioAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ComentarioController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(ComentarioController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new Edible.Web.ViewModels.ComentarioViewModel());
        await using var unitOfWork = _fileUnitOfWorkFactory.Create();
        var fileService = new ComentarioService(unitOfWork);
        await unitOfWork.BeginTransactionAsync("MVC.Comentario.Delete");
        var entity = await fileService.ReadOIDComentarioDefaultAsync(id);
        if (entity is null) { await unitOfWork.RollbackAsync(); return NotFound(); }
        try
        {
            await fileService.DestroyAsync(id);
            await unitOfWork.CommitAsync();
        }
        catch (Exception error)
        {
            await unitOfWork.RollbackAsync();
            _fileLogger.LogError(error, "Could not delete Comentario.");
            ModelState.AddModelError(string.Empty, "No se ha podido eliminar el registro.");
            return View("Delete", ComentarioAssembler.ToViewModel(entity));
        }
        await _files.DeleteEntityFileAsync(entity.Imagen);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(ComentarioController_method_ValidateUploadsAsync) START*/
    private async Task ValidateUploadsAsync(Edible.Web.ViewModels.ComentarioViewModel model, CancellationToken cancellationToken)
    {
        if (true)
        {
            if (model.ImagenUpload is not null)
            {
                try { await _files.ValidateAsync(model.ImagenUpload, cancellationToken); }
                catch (InvalidDataException error) { ModelState.AddModelError(nameof(model.ImagenUpload), error.Message); }
            }
        }
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ComentarioController_method_LoadFormOptionsAsync_EdibleWebViewModelsComentarioViewModel) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.ComentarioViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ComentarioController_method_LoadFormOptionsAsync_EdibleWebViewModelsComentarioViewModel_EdibleApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.ComentarioViewModel model, Edible.Application.Abstractions.IUnitOfWork unitOfWork)
    {

        var options0 = (await unitOfWork.UsuarioRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Name, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options1 = (await unitOfWork.RecetaRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.ComentadorOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.RecetaOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();

    }
    /*PROTECTED REGION END*/

}
