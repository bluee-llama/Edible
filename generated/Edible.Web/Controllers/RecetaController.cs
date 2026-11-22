// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(RecetaController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

/*PROTECTED REGION ID(RecetaController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class RecetaController : Controller
{
    /*PROTECTED REGION ID(RecetaController_dependencies) START*/
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _useCaseUnitOfWorkFactory;
    private readonly RecetaService _recetaService;
    private readonly ILogger<RecetaController> _logger;
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public RecetaController(RecetaService recetaService, Edible.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, Edible.Application.Abstractions.IUnitOfWorkFactory useCaseUnitOfWorkFactory, ILogger<RecetaController>? logger = null)
    {
        _recetaService = recetaService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;
        _useCaseUnitOfWorkFactory = useCaseUnitOfWorkFactory;
        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<RecetaController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(RecetaController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(RecetaController_Index_GET_body) START*/

        var entities = await _recetaService.ReadAllAsync();
        return View(entities.Select(RecetaAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(RecetaController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(RecetaController_Details_GET_body) START*/

        var entity = await _recetaService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(RecetaAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(RecetaController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(RecetaController_Create_GET_body) START*/

        var model = new Edible.Web.ViewModels.RecetaViewModel();
        model.Lineas.Add(new());
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(RecetaController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(Edible.Web.ViewModels.RecetaViewModel model)
    {
    /*PROTECTED REGION ID(RecetaController_Create_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.RecetaService(unitOfWork);



        model.Pasos ??= new();
        model.Lineas ??= new();
        if (model.Pasos.Count > 500) return BadRequest("Número de líneas no válido (máximo 500).");
        if (model.Lineas.Count > 500) return BadRequest("Número de líneas no válido (máximo 500).");
        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ChefId is int selectedChef &&
            !model.ChefOptions.Any(option => option.Value == Convert.ToString(selectedChef, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ChefId", "Selecciona un registro existente de Chef.");
        if (model.ChefId is null) ModelState.AddModelError("ChefId", "Debes seleccionar una opción.");

        if (model.Pasos is null) { model.Pasos = new(); ModelState.AddModelError(nameof(model.Pasos), "Debes indicar las líneas."); }
        if (model.Pasos.Count < 0 || model.Pasos.Count > 500)
            ModelState.AddModelError(nameof(model.Pasos), "Número de líneas no válido (máximo 500).");

        for (var index = 0; index < model.Pasos.Count; index++)
        {
            var row = model.Pasos[index];
            if (!EqualityComparer<int>.Default.Equals(row.Id, default!))
                ModelState.AddModelError($"Pasos[{index}].Id", "La línea no pertenece a este registro o está repetida.");

        }
        if (model.Lineas is null) { model.Lineas = new(); ModelState.AddModelError(nameof(model.Lineas), "Debes indicar las líneas."); }
        if (model.Lineas.Count < 1 || model.Lineas.Count > 500)
            ModelState.AddModelError(nameof(model.Lineas), "Número de líneas no válido (máximo 500).");

        for (var index = 0; index < model.Lineas.Count; index++)
        {
            var row = model.Lineas[index];
            if (!EqualityComparer<int>.Default.Equals(row.Id, default!))
                ModelState.AddModelError($"Lineas[{index}].Id", "La línea no pertenece a este registro o está repetida.");
        if (row.IngredienteCatalogoId is int selectedRowLineasIngredienteCatalogo &&
            !row.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedRowLineasIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"Lineas[{index}].IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (row.IngredienteCatalogoId is null) ModelState.AddModelError($"Lineas[{index}].IngredienteCatalogoId", "Debes seleccionar una opción.");
        }


        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Receta.Create");
            var submittedPasos = await service.PrepareRecetaPasosAsync(null,
                model.Pasos.Select(row => (row.Id, PasoAssembler.Create(row))).ToList());
            var submittedLineas = await service.PrepareRecetaLineasAsync(null,
                model.Lineas.Select(row => (row.Id, LineaRecetaAssembler.Create(row), row.IngredienteCatalogoId)).ToList());

            var entity = await service.NewAsync(model.Nombre, model.Descripcion, model.DietaEspecial, model.ValoracionMedia, model.TiempoRealizacion, model.Dificultad, model.ChefId.GetValueOrDefault(), submittedLineas);

            await service.ApplyRecetaRelationsAsync(entity, p_chef_id: model.ChefId, p_pasos: submittedPasos, p_lineas: submittedLineas);

            await service.ModifierRecetaDefaultAsync(entity);


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
    /*PROTECTED REGION ID(RecetaController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(RecetaController_Edit_GET_body) START*/

        var entity = await _recetaService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = RecetaAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(RecetaController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, Edible.Web.ViewModels.RecetaViewModel model)
    {
    /*PROTECTED REGION ID(RecetaController_Edit_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.RecetaService(unitOfWork);

        var entity = await service.ReadOIDRecetaDefaultAsync(id);
        if (entity is null) return NotFound();

        model.Pasos ??= new();
        model.Lineas ??= new();
        if (model.Pasos.Count > 500) return BadRequest("Número de líneas no válido (máximo 500).");
        if (model.Lineas.Count > 500) return BadRequest("Número de líneas no válido (máximo 500).");
        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.ChefId is int selectedChef &&
            !model.ChefOptions.Any(option => option.Value == Convert.ToString(selectedChef, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ChefId", "Selecciona un registro existente de Chef.");
        if (model.ChefId is null) ModelState.AddModelError("ChefId", "Debes seleccionar una opción.");

        if (model.Pasos is null) { model.Pasos = new(); ModelState.AddModelError(nameof(model.Pasos), "Debes indicar las líneas."); }
        if (model.Pasos.Count < 0 || model.Pasos.Count > 500)
            ModelState.AddModelError(nameof(model.Pasos), "Número de líneas no válido (máximo 500).");
        var seenPasos = new HashSet<int>();
        for (var index = 0; index < model.Pasos.Count; index++)
        {
            var row = model.Pasos[index];
            if (!EqualityComparer<int>.Default.Equals(row.Id, default!) &&
                (!entity.Pasos.Any(child => child.Id.Equals(row.Id)) || !seenPasos.Add(row.Id)))
                ModelState.AddModelError($"Pasos[{index}].Id", "La línea no pertenece a este registro o está repetida.");

        }
        if (model.Lineas is null) { model.Lineas = new(); ModelState.AddModelError(nameof(model.Lineas), "Debes indicar las líneas."); }
        if (model.Lineas.Count < 1 || model.Lineas.Count > 500)
            ModelState.AddModelError(nameof(model.Lineas), "Número de líneas no válido (máximo 500).");
        var seenLineas = new HashSet<int>();
        for (var index = 0; index < model.Lineas.Count; index++)
        {
            var row = model.Lineas[index];
            if (!EqualityComparer<int>.Default.Equals(row.Id, default!) &&
                (!entity.Lineas.Any(child => child.Id.Equals(row.Id)) || !seenLineas.Add(row.Id)))
                ModelState.AddModelError($"Lineas[{index}].Id", "La línea no pertenece a este registro o está repetida.");
        if (row.IngredienteCatalogoId is int selectedRowLineasIngredienteCatalogo &&
            !row.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedRowLineasIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"Lineas[{index}].IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (row.IngredienteCatalogoId is null) ModelState.AddModelError($"Lineas[{index}].IngredienteCatalogoId", "Debes seleccionar una opción.");
        }


        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Receta.Edit");
            var submittedPasos = await service.PrepareRecetaPasosAsync(entity,
                model.Pasos.Select(row => (row.Id, PasoAssembler.Create(row))).ToList());
            var submittedLineas = await service.PrepareRecetaLineasAsync(entity,
                model.Lineas.Select(row => (row.Id, LineaRecetaAssembler.Create(row), row.IngredienteCatalogoId)).ToList());

            await service.ModifyAsync(id, model.Nombre, model.Descripcion, model.DietaEspecial, model.ValoracionMedia, model.TiempoRealizacion, model.Dificultad, model.ChefId.GetValueOrDefault());
            entity = await service.ReadOIDRecetaDefaultAsync(id) ?? throw new InvalidOperationException("Entity disappeared during update.");
            await service.ApplyRecetaRelationsAsync(entity, p_chef_id: model.ChefId, p_pasos: submittedPasos, p_lineas: submittedLineas);

            await service.ModifierRecetaDefaultAsync(entity);


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
    /*PROTECTED REGION ID(RecetaController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(RecetaController_Delete_GET_body) START*/

        var entity = await _recetaService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(RecetaAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(RecetaController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(RecetaController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new Edible.Web.ViewModels.RecetaViewModel());
        await _recetaService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(RecetaController_BusquedaAvanzada_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> BusquedaAvanzada(DietaEspecial p_dieta, float p_notaMinima, int p_tiempoMin, int p_tiempoMax, int p_dificultad, Alergenos p_alergenoExcluido, int p_ingrediente_oid)
    {
    /*PROTECTED REGION ID(RecetaController_BusquedaAvanzada_GET_body) START*/

        var result = await _recetaService.BusquedaAvanzadaAsync(p_dieta, p_notaMinima, p_tiempoMin, p_tiempoMax, p_dificultad, p_alergenoExcluido, p_ingrediente_oid);
        return Ok(result.Select(RecetaAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(RecetaController_BuscarNombre_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> BuscarNombre(string p_nombre)
    {
    /*PROTECTED REGION ID(RecetaController_BuscarNombre_GET_body) START*/

        var result = await _recetaService.BuscarNombreAsync(p_nombre);
        return Ok(result.Select(RecetaAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(RecetaController_RecetasPorNevera_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> RecetasPorNevera(int p_nevera_oid)
    {
    /*PROTECTED REGION ID(RecetaController_RecetasPorNevera_GET_body) START*/

        var result = await _recetaService.RecetasPorNeveraAsync(p_nevera_oid);
        return Ok(result.Select(RecetaAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(RecetaController_method_LoadFormOptionsAsync_EdibleWebViewModelsRecetaViewModel) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.RecetaViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(RecetaController_method_LoadFormOptionsAsync_EdibleWebViewModelsRecetaViewModel_EdibleApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.RecetaViewModel model, Edible.Application.Abstractions.IUnitOfWork unitOfWork)
    {
        if (model.Pasos.Count > 500) { ModelState.AddModelError(nameof(model.Pasos), "Máximo 500 líneas."); return; }
        if (model.Lineas.Count > 500) { ModelState.AddModelError(nameof(model.Lineas), "Máximo 500 líneas."); return; }
        var options0 = (await unitOfWork.ChefRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Name, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options1 = (await unitOfWork.IngredienteRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.ChefOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        foreach (var row in model.Pasos)
        {

        }

        foreach (var row in model.Lineas)
        {
        row.IngredienteCatalogoOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        }
        model.LineasTemplate.IngredienteCatalogoOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
    }
    /*PROTECTED REGION END*/

}
