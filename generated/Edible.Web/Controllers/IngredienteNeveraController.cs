// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(IngredienteNeveraController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

/*PROTECTED REGION ID(IngredienteNeveraController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class IngredienteNeveraController : Controller
{
    /*PROTECTED REGION ID(IngredienteNeveraController_dependencies) START*/
    private readonly IngredienteNeveraService _ingredienteNeveraService;
    private readonly ILogger<IngredienteNeveraController> _logger;
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public IngredienteNeveraController(IngredienteNeveraService ingredienteNeveraService, Edible.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<IngredienteNeveraController>? logger = null)
    {
        _ingredienteNeveraService = ingredienteNeveraService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<IngredienteNeveraController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(IngredienteNeveraController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_Index_GET_body) START*/

        var entities = await _ingredienteNeveraService.ReadAllAsync();
        return View(entities.Select(IngredienteNeveraAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteNeveraController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_Details_GET_body) START*/

        var entity = await _ingredienteNeveraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(IngredienteNeveraAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteNeveraController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_Create_GET_body) START*/

        var model = new Edible.Web.ViewModels.IngredienteNeveraViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteNeveraController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(Edible.Web.ViewModels.IngredienteNeveraViewModel model)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_Create_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.IngredienteNeveraService(unitOfWork);





        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.IngredienteCatalogoId is int selectedIngredienteCatalogo &&
            !model.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (model.IngredienteCatalogoId is null) ModelState.AddModelError("IngredienteCatalogoId", "Debes seleccionar una opción.");
        if (model.NeveraId is int selectedNevera &&
            !model.NeveraOptions.Any(option => option.Value == Convert.ToString(selectedNevera, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("NeveraId", "Selecciona un registro existente de Nevera.");
        if (model.NeveraId is null) ModelState.AddModelError("NeveraId", "Debes seleccionar una opción.");
        if (model.RecetaId is int selectedReceta &&
            !model.RecetaOptions.Any(option => option.Value == Convert.ToString(selectedReceta, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("RecetaId", "Selecciona un registro existente de Receta.");
        if (model.RecetaId is null) ModelState.AddModelError("RecetaId", "Debes seleccionar una opción.");




        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.IngredienteNevera.Create");


            var entity = await service.NewAsync(model.Cantidad, model.FechaCaducidad, model.IngredienteCatalogoId.GetValueOrDefault(), model.NeveraId.GetValueOrDefault(), model.RecetaId.GetValueOrDefault());

            await service.ApplyIngredienteNeveraRelationsAsync(entity, p_ingredienteCatalogo_id: model.IngredienteCatalogoId, p_nevera_id: model.NeveraId, p_receta_id: model.RecetaId);

            await service.ModifierIngredienteNeveraDefaultAsync(entity);


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
    /*PROTECTED REGION ID(IngredienteNeveraController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_Edit_GET_body) START*/

        var entity = await _ingredienteNeveraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = IngredienteNeveraAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteNeveraController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, Edible.Web.ViewModels.IngredienteNeveraViewModel model)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_Edit_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.IngredienteNeveraService(unitOfWork);

        var entity = await service.ReadOIDIngredienteNeveraDefaultAsync(id);
        if (entity is null) return NotFound();



        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.IngredienteCatalogoId is int selectedIngredienteCatalogo &&
            !model.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (model.IngredienteCatalogoId is null) ModelState.AddModelError("IngredienteCatalogoId", "Debes seleccionar una opción.");
        if (model.NeveraId is int selectedNevera &&
            !model.NeveraOptions.Any(option => option.Value == Convert.ToString(selectedNevera, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("NeveraId", "Selecciona un registro existente de Nevera.");
        if (model.NeveraId is null) ModelState.AddModelError("NeveraId", "Debes seleccionar una opción.");
        if (model.RecetaId is int selectedReceta &&
            !model.RecetaOptions.Any(option => option.Value == Convert.ToString(selectedReceta, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("RecetaId", "Selecciona un registro existente de Receta.");
        if (model.RecetaId is null) ModelState.AddModelError("RecetaId", "Debes seleccionar una opción.");




        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.IngredienteNevera.Edit");


            await service.ModifyAsync(id, model.Cantidad, model.FechaCaducidad);
            entity = await service.ReadOIDIngredienteNeveraDefaultAsync(id) ?? throw new InvalidOperationException("Entity disappeared during update.");
            await service.ApplyIngredienteNeveraRelationsAsync(entity, p_ingredienteCatalogo_id: model.IngredienteCatalogoId, p_nevera_id: model.NeveraId, p_receta_id: model.RecetaId);

            await service.ModifierIngredienteNeveraDefaultAsync(entity);


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
    /*PROTECTED REGION ID(IngredienteNeveraController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_Delete_GET_body) START*/

        var entity = await _ingredienteNeveraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(IngredienteNeveraAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteNeveraController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new Edible.Web.ViewModels.IngredienteNeveraViewModel());
        await _ingredienteNeveraService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteNeveraController_FiltrarNevera_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> FiltrarNevera(int p_nevera_oid, TipoIngrediente p_tipo, DateTime p_fechaLimiteCerca, DateTime p_fechaActual, EstadoCaducidad p_estado)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_FiltrarNevera_GET_body) START*/

        var result = await _ingredienteNeveraService.FiltrarNeveraAsync(p_nevera_oid, p_tipo, p_fechaLimiteCerca, p_fechaActual, p_estado);
        return Ok(result.Select(IngredienteNeveraAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(IngredienteNeveraController_method_LoadFormOptionsAsync_EdibleWebViewModelsIngredienteNeveraViewModel) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.IngredienteNeveraViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(IngredienteNeveraController_method_LoadFormOptionsAsync_EdibleWebViewModelsIngredienteNeveraViewModel_EdibleApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.IngredienteNeveraViewModel model, Edible.Application.Abstractions.IUnitOfWork unitOfWork)
    {

        var options0 = (await unitOfWork.IngredienteRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options1 = (await unitOfWork.NeveraRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options2 = (await unitOfWork.RecetaRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.IngredienteCatalogoOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.NeveraOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.RecetaOptions = options2.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();

    }
    /*PROTECTED REGION END*/

}
