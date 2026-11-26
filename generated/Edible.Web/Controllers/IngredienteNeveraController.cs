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
[Authorize(Roles = "Usuario,Chef,Administrador")]
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
    /*PROTECTED REGION ID(IngredienteNeveraController_Index_GET_body) ENABLED START*/

        var entities = await _ingredienteNeveraService.ReadAllAsync();
        if (!IsAdministrador)
        {
            var myNeveraId = await GetCurrentUserNeveraIdAsync();
            entities = entities
                .Where(e => e.Nevera?.Id == myNeveraId)
                .OrderBy(e => e.FechaCaducidad)
                .ToList();
        }
        return View(entities.Select(IngredienteNeveraAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteNeveraController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_Details_GET_body) ENABLED START*/

        var entity = await _ingredienteNeveraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        if (!await CanAccessNeveraItemAsync(entity)) return Forbid();
        return View(IngredienteNeveraAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteNeveraController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_Create_GET_body) ENABLED START*/

        var model = new Edible.Web.ViewModels.IngredienteNeveraViewModel
        {
            Cantidad = 1,
            FechaCaducidad = DateTime.Today.AddDays(7)
        };

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
    /*PROTECTED REGION ID(IngredienteNeveraController_Create_POST_body) ENABLED START*/

        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.IngredienteNeveraService(unitOfWork);

        if (!IsAdministrador)
        {
            model.NeveraId = await GetCurrentUserNeveraIdAsync(unitOfWork);
            model.RecetaId = null;
        }

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

        if (!ModelState.IsValid) return View(model);

        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.IngredienteNevera.Create");

            var ingrediente = await unitOfWork.IngredienteRepository.ReadAsync(model.IngredienteCatalogoId.GetValueOrDefault())
                ?? throw new InvalidOperationException("Ingrediente not found.");
            var nevera = await unitOfWork.NeveraRepository.ReadAsync(model.NeveraId.GetValueOrDefault())
                ?? throw new InvalidOperationException("Nevera not found.");
            Receta? receta = null;
            if (model.RecetaId is int recetaId && recetaId > 0)
            {
                receta = await unitOfWork.RecetaRepository.ReadAsync(recetaId)
                    ?? throw new InvalidOperationException("Receta not found.");
            }

            var entity = new IngredienteNevera
            {
                Cantidad = model.Cantidad,
                FechaCaducidad = model.FechaCaducidad,
                IngredienteCatalogo = ingrediente,
                Nevera = nevera,
                Receta = receta!
            };
            await service.AddIngredienteNeveraDefaultAsync(entity);

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

        TempData["Success"] = "Ingrediente añadido a la nevera.";
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteNeveraController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_Edit_GET_body) ENABLED START*/

        var entity = await _ingredienteNeveraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        if (!await CanAccessNeveraItemAsync(entity)) return Forbid();
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
    /*PROTECTED REGION ID(IngredienteNeveraController_Edit_POST_body) ENABLED START*/

        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.IngredienteNeveraService(unitOfWork);

        var entity = await service.ReadOIDIngredienteNeveraDefaultAsync(id);
        if (entity is null) return NotFound();
        if (!await CanAccessNeveraItemAsync(entity, unitOfWork)) return Forbid();

        if (!IsAdministrador)
        {
            model.NeveraId = entity.Nevera?.Id ?? await GetCurrentUserNeveraIdAsync(unitOfWork);
            model.RecetaId = entity.Receta?.Id;
        }

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

        if (!ModelState.IsValid) return View(model);

        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.IngredienteNevera.Edit");

            await service.ModifyAsync(id, model.Cantidad, model.FechaCaducidad);
            entity = await service.ReadOIDIngredienteNeveraDefaultAsync(id) ?? throw new InvalidOperationException("Entity disappeared during update.");

            entity.IngredienteCatalogo = await unitOfWork.IngredienteRepository.ReadAsync(model.IngredienteCatalogoId.GetValueOrDefault())
                ?? throw new InvalidOperationException("Ingrediente not found.");
            entity.Nevera = await unitOfWork.NeveraRepository.ReadAsync(model.NeveraId.GetValueOrDefault())
                ?? throw new InvalidOperationException("Nevera not found.");
            entity.Receta = model.RecetaId is int recetaId && recetaId > 0
                ? await unitOfWork.RecetaRepository.ReadAsync(recetaId) ?? throw new InvalidOperationException("Receta not found.")
                : null!;

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

        TempData["Success"] = "Ingrediente en nevera actualizado.";
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteNeveraController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_Delete_GET_body) ENABLED START*/

        var entity = await _ingredienteNeveraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        if (!await CanAccessNeveraItemAsync(entity)) return Forbid();
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
    /*PROTECTED REGION ID(IngredienteNeveraController_DeleteConfirmed_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return View("Delete", new Edible.Web.ViewModels.IngredienteNeveraViewModel());
        var entity = await _ingredienteNeveraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        if (!await CanAccessNeveraItemAsync(entity)) return Forbid();
        await _ingredienteNeveraService.DestroyAsync(id);
        TempData["Success"] = "Ingrediente eliminado de la nevera.";
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteNeveraController_FiltrarNevera_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> FiltrarNevera(int p_nevera_oid, TipoIngrediente p_tipo, DateTime p_fechaLimiteCerca, DateTime p_fechaActual, EstadoCaducidad p_estado)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_FiltrarNevera_GET_body) ENABLED START*/

        var model = await BuildFiltrarNeveraAsync(p_nevera_oid, p_tipo, p_fechaLimiteCerca, p_fechaActual, p_estado);
        return View(model);
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(IngredienteNeveraController_method_LoadFormOptionsAsync_EdibleWebViewModelsIngredienteNeveraViewModel) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.IngredienteNeveraViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(IngredienteNeveraController_method_LoadFormOptionsAsync_EdibleWebViewModelsIngredienteNeveraViewModel_EdibleApplicationAbstractionsIUnitOfWork) ENABLED START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.IngredienteNeveraViewModel model, Edible.Application.Abstractions.IUnitOfWork unitOfWork)
    {
        var currentUserId = int.TryParse(User?.Identity?.Name, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var uid) ? uid : (int?)null;
        var neveras = await unitOfWork.NeveraRepository.ReadAllAsync();
        if (model.NeveraId is null && currentUserId is int currentId)
        {
            model.NeveraId = neveras.FirstOrDefault(n => n.Usuario?.Id == currentId)?.Id;
        }

        var options0 = (await unitOfWork.IngredienteRepository.ReadAllAsync()).OrderBy(item => item.Nombre).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options1 = neveras.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = item.Usuario is not null ? $"Nevera #{item.Id} ({item.Usuario.Name})" : $"Nevera #{item.Id}" }).ToList();
        var options2 = (await unitOfWork.RecetaRepository.ReadAllAsync()).OrderBy(item => item.Nombre).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
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
