// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(IngredienteController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

/*PROTECTED REGION ID(IngredienteController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class IngredienteController : Controller
{
    /*PROTECTED REGION ID(IngredienteController_dependencies) START*/
    private readonly IngredienteService _ingredienteService;
    private readonly ILogger<IngredienteController> _logger;
    public IngredienteController(IngredienteService ingredienteService, ILogger<IngredienteController>? logger = null)
    {
        _ingredienteService = ingredienteService;



        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<IngredienteController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(IngredienteController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(IngredienteController_Index_GET_body) START*/

        var entities = await _ingredienteService.ReadAllAsync();
        return View(entities.Select(IngredienteAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(IngredienteController_Details_GET_body) START*/

        var entity = await _ingredienteService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(IngredienteAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public IActionResult Create()
    {
    /*PROTECTED REGION ID(IngredienteController_Create_GET_body) START*/

        return View(new Edible.Web.ViewModels.IngredienteViewModel());
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(Edible.Web.ViewModels.IngredienteViewModel model)
    {
    /*PROTECTED REGION ID(IngredienteController_Create_POST_body) START*/


        if (model.Calorias is null) ModelState.AddModelError(nameof(model.Calorias), "Debes indicar Calorias.");
        if (model.G_proteinas is null) ModelState.AddModelError(nameof(model.G_proteinas), "Debes indicar G_proteinas.");
        if (model.G_carbohidratos is null) ModelState.AddModelError(nameof(model.G_carbohidratos), "Debes indicar G_carbohidratos.");
        if (model.G_grasas is null) ModelState.AddModelError(nameof(model.G_grasas), "Debes indicar G_grasas.");
        if (model.G_racion is null) ModelState.AddModelError(nameof(model.G_racion), "Debes indicar G_racion.");

        if (!ModelState.IsValid) return View(model);

        try
        {
            await _ingredienteService.NewAsync(model.Nombre, model.Tipo, model.Calorias.GetValueOrDefault(), model.G_proteinas.GetValueOrDefault(), model.G_carbohidratos.GetValueOrDefault(), model.G_grasas.GetValueOrDefault(), model.G_racion.GetValueOrDefault(), (model.Alergenos ?? new()).ToList(), (model.DietaEspecial ?? new()).ToList());
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            _logger.LogError(error, "Could not create the record.");
            ModelState.AddModelError(string.Empty, "No se ha podido guardar el registro. Inténtalo de nuevo.");
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(IngredienteController_Edit_GET_body) START*/

        var entity = await _ingredienteService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(IngredienteAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, Edible.Web.ViewModels.IngredienteViewModel model)
    {
    /*PROTECTED REGION ID(IngredienteController_Edit_POST_body) START*/


        if (model.Calorias is null) ModelState.AddModelError(nameof(model.Calorias), "Debes indicar Calorias.");
        if (model.G_proteinas is null) ModelState.AddModelError(nameof(model.G_proteinas), "Debes indicar G_proteinas.");
        if (model.G_carbohidratos is null) ModelState.AddModelError(nameof(model.G_carbohidratos), "Debes indicar G_carbohidratos.");
        if (model.G_grasas is null) ModelState.AddModelError(nameof(model.G_grasas), "Debes indicar G_grasas.");
        if (model.G_racion is null) ModelState.AddModelError(nameof(model.G_racion), "Debes indicar G_racion.");
        if (!ModelState.IsValid) return View(model);

        try
        {
            await _ingredienteService.ModifyAsync(id, model.Nombre, model.Tipo, model.Calorias.GetValueOrDefault(), model.G_proteinas.GetValueOrDefault(), model.G_carbohidratos.GetValueOrDefault(), model.G_grasas.GetValueOrDefault(), model.G_racion.GetValueOrDefault(), (model.Alergenos ?? new()).ToList(), (model.DietaEspecial ?? new()).ToList());
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            _logger.LogError(error, "Could not update the record.");
            ModelState.AddModelError(string.Empty, "No se ha podido guardar el registro. Inténtalo de nuevo.");
            return View(model);
        }
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(IngredienteController_Delete_GET_body) START*/

        var entity = await _ingredienteService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(IngredienteAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(IngredienteController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new Edible.Web.ViewModels.IngredienteViewModel());
        await _ingredienteService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }
}
