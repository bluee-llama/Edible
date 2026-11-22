// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(LineaRecetaController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

/*PROTECTED REGION ID(LineaRecetaController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class LineaRecetaController : Controller
{
    /*PROTECTED REGION ID(LineaRecetaController_dependencies) START*/
    private readonly RecetaService _recetaService;
    private readonly ILogger<LineaRecetaController> _logger;
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public LineaRecetaController(RecetaService recetaService, Edible.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<LineaRecetaController>? logger = null)
    {
        _recetaService = recetaService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<LineaRecetaController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(LineaRecetaController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(LineaRecetaController_Index_GET_body) START*/

        var entities = await _recetaService.ReadAllLineaRecetaAsync();
        return View(entities.Select(LineaRecetaAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(LineaRecetaController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(LineaRecetaController_Details_GET_body) START*/

        var entity = await _recetaService.ReadOIDLineaRecetaAsync(id);
        if (entity is null) return NotFound();
        return View(LineaRecetaAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(LineaRecetaController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(LineaRecetaController_Create_GET_body) START*/

        var model = new Edible.Web.ViewModels.LineaRecetaViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(LineaRecetaController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(Edible.Web.ViewModels.LineaRecetaViewModel model, int p_ingrediente_oid)
    {
    /*PROTECTED REGION ID(LineaRecetaController_Create_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.RecetaService(unitOfWork);





        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.RecetaId is int selectedReceta &&
            !model.RecetaOptions.Any(option => option.Value == Convert.ToString(selectedReceta, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("RecetaId", "Selecciona un registro existente de Receta.");
        if (model.RecetaId is null) ModelState.AddModelError("RecetaId", "Debes seleccionar una opción.");
        if (model.IngredienteCatalogoId is int selectedIngredienteCatalogo &&
            !model.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (model.IngredienteCatalogoId is null) ModelState.AddModelError("IngredienteCatalogoId", "Debes seleccionar una opción.");




        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.LineaReceta.Create");


            var entity = await service.NewLineaRecetaAsync(model.CantidadNecesaria, model.RecetaId.GetValueOrDefault(), p_ingrediente_oid);

            await service.ApplyLineaRecetaRelationsAsync(entity, p_receta_id: model.RecetaId, p_ingredienteCatalogo_id: model.IngredienteCatalogoId);

            await unitOfWork.LineaRecetaRepository.SaveAsync(entity);


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
    /*PROTECTED REGION ID(LineaRecetaController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(LineaRecetaController_Edit_GET_body) START*/

        var entity = await _recetaService.ReadOIDLineaRecetaAsync(id);
        if (entity is null) return NotFound();
        var model = LineaRecetaAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(LineaRecetaController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, Edible.Web.ViewModels.LineaRecetaViewModel model, int p_ingrediente_oid)
    {
    /*PROTECTED REGION ID(LineaRecetaController_Edit_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.RecetaService(unitOfWork);

        var entity = await unitOfWork.LineaRecetaRepository.ReadAsync(id);
        if (entity is null) return NotFound();



        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.RecetaId is int selectedReceta &&
            !model.RecetaOptions.Any(option => option.Value == Convert.ToString(selectedReceta, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("RecetaId", "Selecciona un registro existente de Receta.");
        if (model.RecetaId is null) ModelState.AddModelError("RecetaId", "Debes seleccionar una opción.");
        if (model.IngredienteCatalogoId is int selectedIngredienteCatalogo &&
            !model.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (model.IngredienteCatalogoId is null) ModelState.AddModelError("IngredienteCatalogoId", "Debes seleccionar una opción.");




        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.LineaReceta.Edit");


            await service.ModifyLineaRecetaAsync(id, model.CantidadNecesaria, model.RecetaId.GetValueOrDefault(), p_ingrediente_oid);
            entity = await unitOfWork.LineaRecetaRepository.ReadAsync(id) ?? throw new InvalidOperationException("Entity disappeared during update.");
            await service.ApplyLineaRecetaRelationsAsync(entity, p_receta_id: model.RecetaId, p_ingredienteCatalogo_id: model.IngredienteCatalogoId);

            await unitOfWork.LineaRecetaRepository.SaveAsync(entity);


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
    /*PROTECTED REGION ID(LineaRecetaController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(LineaRecetaController_Delete_GET_body) START*/

        var entity = await _recetaService.ReadOIDLineaRecetaAsync(id);
        if (entity is null) return NotFound();
        return View(LineaRecetaAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(LineaRecetaController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(LineaRecetaController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new Edible.Web.ViewModels.LineaRecetaViewModel());
        await _recetaService.DestroyLineaRecetaAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(LineaRecetaController_method_LoadFormOptionsAsync_EdibleWebViewModelsLineaRecetaViewModel) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.LineaRecetaViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(LineaRecetaController_method_LoadFormOptionsAsync_EdibleWebViewModelsLineaRecetaViewModel_EdibleApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.LineaRecetaViewModel model, Edible.Application.Abstractions.IUnitOfWork unitOfWork)
    {

        var options0 = (await unitOfWork.RecetaRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options1 = (await unitOfWork.IngredienteRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.RecetaOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.IngredienteCatalogoOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();

    }
    /*PROTECTED REGION END*/

}
