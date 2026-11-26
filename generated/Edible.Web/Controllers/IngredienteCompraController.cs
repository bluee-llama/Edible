// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(IngredienteCompraController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

/*PROTECTED REGION ID(IngredienteCompraController_class_attributes) ENABLED START*/
[Authorize(Roles = "Usuario,Chef,Administrador")]
/*PROTECTED REGION END*/
public partial class IngredienteCompraController : Controller
{
    /*PROTECTED REGION ID(IngredienteCompraController_dependencies) START*/
    private readonly IngredienteCompraService _ingredienteCompraService;
    private readonly ILogger<IngredienteCompraController> _logger;
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public IngredienteCompraController(IngredienteCompraService ingredienteCompraService, Edible.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<IngredienteCompraController>? logger = null)
    {
        _ingredienteCompraService = ingredienteCompraService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<IngredienteCompraController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(IngredienteCompraController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(IngredienteCompraController_Index_GET_body) ENABLED START*/

        var entities = await _ingredienteCompraService.ReadAllAsync();
        if (!IsAdministrador)
        {
            var (_, myListaId) = await GetCurrentUserNeveraAndListaAsync();
            entities = entities
                .Where(e => e.ListaCompra?.Id == myListaId)
                .OrderBy(e => e.Comprado)
                .ThenBy(e => e.IngredienteCatalogo?.Nombre)
                .ToList();
        }
        return View(entities.Select(IngredienteCompraAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteCompraController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(IngredienteCompraController_Details_GET_body) ENABLED START*/

        var entity = await _ingredienteCompraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        if (!await CanAccessCompraItemAsync(entity)) return Forbid();
        return View(IngredienteCompraAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteCompraController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(IngredienteCompraController_Create_GET_body) ENABLED START*/

        var model = new Edible.Web.ViewModels.IngredienteCompraViewModel
        {
            Cantidad = 1
        };

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteCompraController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(Edible.Web.ViewModels.IngredienteCompraViewModel model, DateTime p_fechaCaducidad)
    {
    /*PROTECTED REGION ID(IngredienteCompraController_Create_POST_body) ENABLED START*/

        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.IngredienteCompraService(unitOfWork);

        if (!IsAdministrador)
        {
            var (myNeveraId, myListaId) = await GetCurrentUserNeveraAndListaAsync(unitOfWork);
            model.NeveraId = myNeveraId;
            model.ListaCompraId = myListaId;
        }

        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.NeveraId is int selectedNevera &&
            !model.NeveraOptions.Any(option => option.Value == Convert.ToString(selectedNevera, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("NeveraId", "Selecciona un registro existente de Nevera.");
        if (model.NeveraId is null) ModelState.AddModelError("NeveraId", "Debes seleccionar una opción.");
        if (model.ListaCompraId is int selectedListaCompra &&
            !model.ListaCompraOptions.Any(option => option.Value == Convert.ToString(selectedListaCompra, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ListaCompraId", "Selecciona un registro existente de ListaCompra.");
        if (model.ListaCompraId is null) ModelState.AddModelError("ListaCompraId", "Debes seleccionar una opción.");
        if (model.IngredienteCatalogoId is int selectedIngredienteCatalogo &&
            !model.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (model.IngredienteCatalogoId is null) ModelState.AddModelError("IngredienteCatalogoId", "Debes seleccionar una opción.");

        if (!ModelState.IsValid) return View(model);

        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.IngredienteCompra.Create");

            var entity = await service.NewAsync(model.Cantidad, p_fechaCaducidad, model.Comprado, model.NeveraId.GetValueOrDefault(), model.ListaCompraId.GetValueOrDefault(), model.IngredienteCatalogoId.GetValueOrDefault());

            await service.ApplyIngredienteCompraRelationsAsync(entity, p_nevera_id: model.NeveraId, p_listaCompra_id: model.ListaCompraId, p_ingredienteCatalogo_id: model.IngredienteCatalogoId);

            await service.ModifierIngredienteCompraDefaultAsync(entity);

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

        TempData["Success"] = "Producto añadido a la lista de la compra.";
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteCompraController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(IngredienteCompraController_Edit_GET_body) ENABLED START*/

        var entity = await _ingredienteCompraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        if (!await CanAccessCompraItemAsync(entity)) return Forbid();
        var model = IngredienteCompraAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteCompraController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, Edible.Web.ViewModels.IngredienteCompraViewModel model, DateTime p_fechaCaducidad)
    {
    /*PROTECTED REGION ID(IngredienteCompraController_Edit_POST_body) ENABLED START*/

        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.IngredienteCompraService(unitOfWork);

        var entity = await service.ReadOIDIngredienteCompraDefaultAsync(id);
        if (entity is null) return NotFound();
        if (!await CanAccessCompraItemAsync(entity, unitOfWork)) return Forbid();

        if (!IsAdministrador)
        {
            var (myNeveraId, myListaId) = await GetCurrentUserNeveraAndListaAsync(unitOfWork);
            model.NeveraId = entity.Nevera?.Id ?? myNeveraId;
            model.ListaCompraId = entity.ListaCompra?.Id ?? myListaId;
        }

        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.NeveraId is int selectedNevera &&
            !model.NeveraOptions.Any(option => option.Value == Convert.ToString(selectedNevera, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("NeveraId", "Selecciona un registro existente de Nevera.");
        if (model.NeveraId is null) ModelState.AddModelError("NeveraId", "Debes seleccionar una opción.");
        if (model.ListaCompraId is int selectedListaCompra &&
            !model.ListaCompraOptions.Any(option => option.Value == Convert.ToString(selectedListaCompra, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ListaCompraId", "Selecciona un registro existente de ListaCompra.");
        if (model.ListaCompraId is null) ModelState.AddModelError("ListaCompraId", "Debes seleccionar una opción.");
        if (model.IngredienteCatalogoId is int selectedIngredienteCatalogo &&
            !model.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (model.IngredienteCatalogoId is null) ModelState.AddModelError("IngredienteCatalogoId", "Debes seleccionar una opción.");

        if (!ModelState.IsValid) return View(model);

        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.IngredienteCompra.Edit");

            await service.ModifyAsync(id, model.Cantidad, p_fechaCaducidad, model.Comprado, model.NeveraId.GetValueOrDefault(), model.ListaCompraId.GetValueOrDefault(), model.IngredienteCatalogoId.GetValueOrDefault());
            entity = await service.ReadOIDIngredienteCompraDefaultAsync(id) ?? throw new InvalidOperationException("Entity disappeared during update.");
            await service.ApplyIngredienteCompraRelationsAsync(entity, p_nevera_id: model.NeveraId, p_listaCompra_id: model.ListaCompraId, p_ingredienteCatalogo_id: model.IngredienteCatalogoId);

            await service.ModifierIngredienteCompraDefaultAsync(entity);

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

        TempData["Success"] = "Producto de la lista actualizado.";
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(IngredienteCompraController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(IngredienteCompraController_Delete_GET_body) ENABLED START*/

        var entity = await _ingredienteCompraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        if (!await CanAccessCompraItemAsync(entity)) return Forbid();
        return View(IngredienteCompraAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteCompraController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(IngredienteCompraController_DeleteConfirmed_POST_body) ENABLED START*/

        if (!ModelState.IsValid) return View("Delete", new Edible.Web.ViewModels.IngredienteCompraViewModel());
        var entity = await _ingredienteCompraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        if (!await CanAccessCompraItemAsync(entity)) return Forbid();
        await _ingredienteCompraService.DestroyAsync(id);
        TempData["Success"] = "Producto eliminado de la lista de la compra.";
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(IngredienteCompraController_method_LoadFormOptionsAsync_EdibleWebViewModelsIngredienteCompraViewModel) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.IngredienteCompraViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(IngredienteCompraController_method_LoadFormOptionsAsync_EdibleWebViewModelsIngredienteCompraViewModel_EdibleApplicationAbstractionsIUnitOfWork) ENABLED START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.IngredienteCompraViewModel model, Edible.Application.Abstractions.IUnitOfWork unitOfWork)
    {
        var currentUserId = int.TryParse(User?.Identity?.Name, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var uid) ? uid : (int?)null;
        var neveras = await unitOfWork.NeveraRepository.ReadAllAsync();
        var listas = await unitOfWork.ListaCompraRepository.ReadAllAsync();
        if (currentUserId is int currentId)
        {
            model.NeveraId ??= neveras.FirstOrDefault(n => n.Usuario?.Id == currentId)?.Id;
            model.ListaCompraId ??= listas.FirstOrDefault(l => l.Usuario?.Id == currentId)?.Id;
        }

        var options0 = neveras.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = item.Usuario is not null ? $"Nevera #{item.Id} ({item.Usuario.Name})" : $"Nevera #{item.Id}" }).ToList();
        var options1 = listas.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = item.Usuario is not null ? $"Lista #{item.Id} ({item.Usuario.Name})" : $"Lista #{item.Id}" }).ToList();
        var options2 = (await unitOfWork.IngredienteRepository.ReadAllAsync()).OrderBy(item => item.Nombre).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.NeveraOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.ListaCompraOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.IngredienteCatalogoOptions = options2.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();

    }
    /*PROTECTED REGION END*/

}
