// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(PasoController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

/*PROTECTED REGION ID(PasoController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class PasoController : Controller
{
    /*PROTECTED REGION ID(PasoController_dependencies) START*/
    private readonly PasoService _pasoService;
    private readonly ILogger<PasoController> _logger;
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public PasoController(PasoService pasoService, Edible.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<PasoController>? logger = null)
    {
        _pasoService = pasoService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<PasoController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(PasoController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(PasoController_Index_GET_body) START*/

        var entities = await _pasoService.ReadAllAsync();
        return View(entities.Select(PasoAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(PasoController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(PasoController_Details_GET_body) START*/

        var entity = await _pasoService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(PasoAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(PasoController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(PasoController_Create_GET_body) START*/

        var model = new Edible.Web.ViewModels.PasoViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(PasoController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(Edible.Web.ViewModels.PasoViewModel model)
    {
    /*PROTECTED REGION ID(PasoController_Create_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.PasoService(unitOfWork);





        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.RecetaId is int selectedReceta &&
            !model.RecetaOptions.Any(option => option.Value == Convert.ToString(selectedReceta, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("RecetaId", "Selecciona un registro existente de Receta.");
        if (model.RecetaId is null) ModelState.AddModelError("RecetaId", "Debes seleccionar una opción.");




        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Paso.Create");


            var entity = await service.NewAsync(model.Nombre, model.Descripcion, model.RecetaId.GetValueOrDefault());

            await service.ApplyPasoRelationsAsync(entity, p_receta_id: model.RecetaId);

            await service.ModifierPasoDefaultAsync(entity);


            await unitOfWork.CommitAsync();
        }
        catch (Exception error)
        {
            await unitOfWork.RollbackAsync();
            if (error is OperationCanceledException) throw;
            _logger.LogError(error, "Could not save the record and its lines.");
            ModelState.AddModelError(string.Empty, "No se han podido guardar el registro y sus líneas.");
            return View(model);
        }


        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(PasoController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(PasoController_Edit_GET_body) START*/

        var entity = await _pasoService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = PasoAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(PasoController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, Edible.Web.ViewModels.PasoViewModel model)
    {
    /*PROTECTED REGION ID(PasoController_Edit_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.PasoService(unitOfWork);

        var entity = await service.ReadOIDPasoDefaultAsync(id);
        if (entity is null) return NotFound();



        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.RecetaId is int selectedReceta &&
            !model.RecetaOptions.Any(option => option.Value == Convert.ToString(selectedReceta, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("RecetaId", "Selecciona un registro existente de Receta.");
        if (model.RecetaId is null) ModelState.AddModelError("RecetaId", "Debes seleccionar una opción.");




        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Paso.Edit");


            await service.ModifyAsync(id, model.Nombre, model.Descripcion);
            entity = await service.ReadOIDPasoDefaultAsync(id) ?? throw new InvalidOperationException("Entity disappeared during update.");
            await service.ApplyPasoRelationsAsync(entity, p_receta_id: model.RecetaId);

            await service.ModifierPasoDefaultAsync(entity);


            await unitOfWork.CommitAsync();
        }
        catch (Exception error)
        {
            await unitOfWork.RollbackAsync();
            if (error is OperationCanceledException) throw;
            _logger.LogError(error, "Could not save the record and its lines.");
            ModelState.AddModelError(string.Empty, "No se han podido guardar el registro y sus líneas.");
            return View(model);
        }


        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(PasoController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(PasoController_Delete_GET_body) START*/

        var entity = await _pasoService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(PasoAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(PasoController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(PasoController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new Edible.Web.ViewModels.PasoViewModel());
        await _pasoService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(PasoController_method_LoadFormOptionsAsync_EdibleWebViewModelsPasoViewModel) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.PasoViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(PasoController_method_LoadFormOptionsAsync_EdibleWebViewModelsPasoViewModel_EdibleApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.PasoViewModel model, Edible.Application.Abstractions.IUnitOfWork unitOfWork)
    {

        var options0 = (await unitOfWork.RecetaRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.RecetaOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();

    }
    /*PROTECTED REGION END*/

}
