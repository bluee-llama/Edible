// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(NeveraController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

/*PROTECTED REGION ID(NeveraController_class_attributes) ENABLED START*/
[Authorize]
/*PROTECTED REGION END*/
public partial class NeveraController : Controller
{
    /*PROTECTED REGION ID(NeveraController_dependencies) START*/
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _useCaseUnitOfWorkFactory;
    private readonly NeveraService _neveraService;
    private readonly ILogger<NeveraController> _logger;
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public NeveraController(NeveraService neveraService, Edible.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, Edible.Application.Abstractions.IUnitOfWorkFactory useCaseUnitOfWorkFactory, ILogger<NeveraController>? logger = null)
    {
        _neveraService = neveraService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;
        _useCaseUnitOfWorkFactory = useCaseUnitOfWorkFactory;
        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<NeveraController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(NeveraController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(NeveraController_Index_GET_body) START*/

        var entities = await _neveraService.ReadAllAsync();
        return View(entities.Select(NeveraAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(NeveraController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(NeveraController_Details_GET_body) START*/

        var entity = await _neveraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(NeveraAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(NeveraController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(NeveraController_Create_GET_body) START*/

        var model = new Edible.Web.ViewModels.NeveraViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(NeveraController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(Edible.Web.ViewModels.NeveraViewModel model)
    {
    /*PROTECTED REGION ID(NeveraController_Create_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.NeveraService(unitOfWork);



        model.IngredienteEnLaLista ??= new();
        model.IngredienteEnLaNevera ??= new();
        if (model.IngredienteEnLaLista.Count > 500) return BadRequest("Número de líneas no válido (máximo 500).");
        if (model.IngredienteEnLaNevera.Count > 500) return BadRequest("Número de líneas no válido (máximo 500).");
        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.UsuarioId is int selectedUsuario &&
            !model.UsuarioOptions.Any(option => option.Value == Convert.ToString(selectedUsuario, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("UsuarioId", "Selecciona un registro existente de Usuario.");
        if (model.UsuarioId is null) ModelState.AddModelError("UsuarioId", "Debes seleccionar una opción.");

        if (model.IngredienteEnLaLista is null) { model.IngredienteEnLaLista = new(); ModelState.AddModelError(nameof(model.IngredienteEnLaLista), "Debes indicar las líneas."); }
        if (model.IngredienteEnLaLista.Count < 0 || model.IngredienteEnLaLista.Count > 500)
            ModelState.AddModelError(nameof(model.IngredienteEnLaLista), "Número de líneas no válido (máximo 500).");

        for (var index = 0; index < model.IngredienteEnLaLista.Count; index++)
        {
            var row = model.IngredienteEnLaLista[index];
            if (!EqualityComparer<int>.Default.Equals(row.Id, default!))
                ModelState.AddModelError($"IngredienteEnLaLista[{index}].Id", "La línea no pertenece a este registro o está repetida.");
        if (row.ListaCompraId is int selectedRowIngredienteEnLaListaListaCompra &&
            !row.ListaCompraOptions.Any(option => option.Value == Convert.ToString(selectedRowIngredienteEnLaListaListaCompra, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"IngredienteEnLaLista[{index}].ListaCompraId", "Selecciona un registro existente de ListaCompra.");
        if (row.ListaCompraId is null) ModelState.AddModelError($"IngredienteEnLaLista[{index}].ListaCompraId", "Debes seleccionar una opción.");
        if (row.IngredienteCatalogoId is int selectedRowIngredienteEnLaListaIngredienteCatalogo &&
            !row.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedRowIngredienteEnLaListaIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"IngredienteEnLaLista[{index}].IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (row.IngredienteCatalogoId is null) ModelState.AddModelError($"IngredienteEnLaLista[{index}].IngredienteCatalogoId", "Debes seleccionar una opción.");
        }
        if (model.IngredienteEnLaNevera is null) { model.IngredienteEnLaNevera = new(); ModelState.AddModelError(nameof(model.IngredienteEnLaNevera), "Debes indicar las líneas."); }
        if (model.IngredienteEnLaNevera.Count < 0 || model.IngredienteEnLaNevera.Count > 500)
            ModelState.AddModelError(nameof(model.IngredienteEnLaNevera), "Número de líneas no válido (máximo 500).");

        for (var index = 0; index < model.IngredienteEnLaNevera.Count; index++)
        {
            var row = model.IngredienteEnLaNevera[index];
            if (!EqualityComparer<int>.Default.Equals(row.Id, default!))
                ModelState.AddModelError($"IngredienteEnLaNevera[{index}].Id", "La línea no pertenece a este registro o está repetida.");
        if (row.IngredienteCatalogoId is int selectedRowIngredienteEnLaNeveraIngredienteCatalogo &&
            !row.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedRowIngredienteEnLaNeveraIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"IngredienteEnLaNevera[{index}].IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (row.IngredienteCatalogoId is null) ModelState.AddModelError($"IngredienteEnLaNevera[{index}].IngredienteCatalogoId", "Debes seleccionar una opción.");
        if (row.RecetaId is int selectedRowIngredienteEnLaNeveraReceta &&
            !row.RecetaOptions.Any(option => option.Value == Convert.ToString(selectedRowIngredienteEnLaNeveraReceta, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"IngredienteEnLaNevera[{index}].RecetaId", "Selecciona un registro existente de Receta.");
        if (row.RecetaId is null) ModelState.AddModelError($"IngredienteEnLaNevera[{index}].RecetaId", "Debes seleccionar una opción.");
        }


        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Nevera.Create");
            var submittedIngredienteEnLaLista = await service.PrepareNeveraIngredienteEnLaListaAsync(null,
                model.IngredienteEnLaLista.Select(row => (row.Id, IngredienteCompraAssembler.Create(row), row.IngredienteCatalogoId, row.ListaCompraId)).ToList());
            var submittedIngredienteEnLaNevera = await service.PrepareNeveraIngredienteEnLaNeveraAsync(null,
                model.IngredienteEnLaNevera.Select(row => (row.Id, IngredienteNeveraAssembler.Create(row), row.IngredienteCatalogoId, row.RecetaId)).ToList());

            var entity = await service.NewAsync(model.UsuarioId.GetValueOrDefault());

            await service.ApplyNeveraRelationsAsync(entity, p_usuario_id: model.UsuarioId, p_ingredienteEnLaLista: submittedIngredienteEnLaLista, p_ingredienteEnLaNevera: submittedIngredienteEnLaNevera);

            await service.ModifierNeveraDefaultAsync(entity);


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
    /*PROTECTED REGION ID(NeveraController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(NeveraController_Edit_GET_body) START*/

        var entity = await _neveraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = NeveraAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(NeveraController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, Edible.Web.ViewModels.NeveraViewModel model)
    {
    /*PROTECTED REGION ID(NeveraController_Edit_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.NeveraService(unitOfWork);

        var entity = await service.ReadOIDNeveraDefaultAsync(id);
        if (entity is null) return NotFound();

        model.IngredienteEnLaLista ??= new();
        model.IngredienteEnLaNevera ??= new();
        if (model.IngredienteEnLaLista.Count > 500) return BadRequest("Número de líneas no válido (máximo 500).");
        if (model.IngredienteEnLaNevera.Count > 500) return BadRequest("Número de líneas no válido (máximo 500).");
        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.UsuarioId is int selectedUsuario &&
            !model.UsuarioOptions.Any(option => option.Value == Convert.ToString(selectedUsuario, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("UsuarioId", "Selecciona un registro existente de Usuario.");
        if (model.UsuarioId is null) ModelState.AddModelError("UsuarioId", "Debes seleccionar una opción.");

        if (model.IngredienteEnLaLista is null) { model.IngredienteEnLaLista = new(); ModelState.AddModelError(nameof(model.IngredienteEnLaLista), "Debes indicar las líneas."); }
        if (model.IngredienteEnLaLista.Count < 0 || model.IngredienteEnLaLista.Count > 500)
            ModelState.AddModelError(nameof(model.IngredienteEnLaLista), "Número de líneas no válido (máximo 500).");
        var seenIngredienteEnLaLista = new HashSet<int>();
        for (var index = 0; index < model.IngredienteEnLaLista.Count; index++)
        {
            var row = model.IngredienteEnLaLista[index];
            if (!EqualityComparer<int>.Default.Equals(row.Id, default!) &&
                (!entity.IngredienteEnLaLista.Any(child => child.Id.Equals(row.Id)) || !seenIngredienteEnLaLista.Add(row.Id)))
                ModelState.AddModelError($"IngredienteEnLaLista[{index}].Id", "La línea no pertenece a este registro o está repetida.");
        if (row.ListaCompraId is int selectedRowIngredienteEnLaListaListaCompra &&
            !row.ListaCompraOptions.Any(option => option.Value == Convert.ToString(selectedRowIngredienteEnLaListaListaCompra, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"IngredienteEnLaLista[{index}].ListaCompraId", "Selecciona un registro existente de ListaCompra.");
        if (row.ListaCompraId is null) ModelState.AddModelError($"IngredienteEnLaLista[{index}].ListaCompraId", "Debes seleccionar una opción.");
        if (row.IngredienteCatalogoId is int selectedRowIngredienteEnLaListaIngredienteCatalogo &&
            !row.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedRowIngredienteEnLaListaIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"IngredienteEnLaLista[{index}].IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (row.IngredienteCatalogoId is null) ModelState.AddModelError($"IngredienteEnLaLista[{index}].IngredienteCatalogoId", "Debes seleccionar una opción.");
        }
        if (model.IngredienteEnLaNevera is null) { model.IngredienteEnLaNevera = new(); ModelState.AddModelError(nameof(model.IngredienteEnLaNevera), "Debes indicar las líneas."); }
        if (model.IngredienteEnLaNevera.Count < 0 || model.IngredienteEnLaNevera.Count > 500)
            ModelState.AddModelError(nameof(model.IngredienteEnLaNevera), "Número de líneas no válido (máximo 500).");
        var seenIngredienteEnLaNevera = new HashSet<int>();
        for (var index = 0; index < model.IngredienteEnLaNevera.Count; index++)
        {
            var row = model.IngredienteEnLaNevera[index];
            if (!EqualityComparer<int>.Default.Equals(row.Id, default!) &&
                (!entity.IngredienteEnLaNevera.Any(child => child.Id.Equals(row.Id)) || !seenIngredienteEnLaNevera.Add(row.Id)))
                ModelState.AddModelError($"IngredienteEnLaNevera[{index}].Id", "La línea no pertenece a este registro o está repetida.");
        if (row.IngredienteCatalogoId is int selectedRowIngredienteEnLaNeveraIngredienteCatalogo &&
            !row.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedRowIngredienteEnLaNeveraIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"IngredienteEnLaNevera[{index}].IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (row.IngredienteCatalogoId is null) ModelState.AddModelError($"IngredienteEnLaNevera[{index}].IngredienteCatalogoId", "Debes seleccionar una opción.");
        if (row.RecetaId is int selectedRowIngredienteEnLaNeveraReceta &&
            !row.RecetaOptions.Any(option => option.Value == Convert.ToString(selectedRowIngredienteEnLaNeveraReceta, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"IngredienteEnLaNevera[{index}].RecetaId", "Selecciona un registro existente de Receta.");
        if (row.RecetaId is null) ModelState.AddModelError($"IngredienteEnLaNevera[{index}].RecetaId", "Debes seleccionar una opción.");
        }


        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Nevera.Edit");
            var submittedIngredienteEnLaLista = await service.PrepareNeveraIngredienteEnLaListaAsync(entity,
                model.IngredienteEnLaLista.Select(row => (row.Id, IngredienteCompraAssembler.Create(row), row.IngredienteCatalogoId, row.ListaCompraId)).ToList());
            var submittedIngredienteEnLaNevera = await service.PrepareNeveraIngredienteEnLaNeveraAsync(entity,
                model.IngredienteEnLaNevera.Select(row => (row.Id, IngredienteNeveraAssembler.Create(row), row.IngredienteCatalogoId, row.RecetaId)).ToList());

            await service.ModifyAsync(id);
            entity = await service.ReadOIDNeveraDefaultAsync(id) ?? throw new InvalidOperationException("Entity disappeared during update.");
            await service.ApplyNeveraRelationsAsync(entity, p_usuario_id: model.UsuarioId, p_ingredienteEnLaLista: submittedIngredienteEnLaLista, p_ingredienteEnLaNevera: submittedIngredienteEnLaNevera);

            await service.ModifierNeveraDefaultAsync(entity);


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
    /*PROTECTED REGION ID(NeveraController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(NeveraController_Delete_GET_body) START*/

        var entity = await _neveraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(NeveraAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(NeveraController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(NeveraController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new Edible.Web.ViewModels.NeveraViewModel());
        await _neveraService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(NeveraController_method_LoadFormOptionsAsync_EdibleWebViewModelsNeveraViewModel) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.NeveraViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(NeveraController_method_LoadFormOptionsAsync_EdibleWebViewModelsNeveraViewModel_EdibleApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.NeveraViewModel model, Edible.Application.Abstractions.IUnitOfWork unitOfWork)
    {
        if (model.IngredienteEnLaLista.Count > 500) { ModelState.AddModelError(nameof(model.IngredienteEnLaLista), "Máximo 500 líneas."); return; }
        if (model.IngredienteEnLaNevera.Count > 500) { ModelState.AddModelError(nameof(model.IngredienteEnLaNevera), "Máximo 500 líneas."); return; }
        var options0 = (await unitOfWork.UsuarioRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Name, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options1 = (await unitOfWork.ListaCompraRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options2 = (await unitOfWork.IngredienteRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options3 = (await unitOfWork.RecetaRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.UsuarioOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        foreach (var row in model.IngredienteEnLaLista)
        {
        row.ListaCompraOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        row.IngredienteCatalogoOptions = options2.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        }
        model.IngredienteEnLaListaTemplate.ListaCompraOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.IngredienteEnLaListaTemplate.IngredienteCatalogoOptions = options2.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        foreach (var row in model.IngredienteEnLaNevera)
        {
        row.IngredienteCatalogoOptions = options2.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        row.RecetaOptions = options3.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        }
        model.IngredienteEnLaNeveraTemplate.IngredienteCatalogoOptions = options2.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.IngredienteEnLaNeveraTemplate.RecetaOptions = options3.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
    }
    /*PROTECTED REGION END*/

}
