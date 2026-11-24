// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(ListaCompraController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

/*PROTECTED REGION ID(ListaCompraController_class_attributes) ENABLED START*/
[Authorize(Roles = "Usuario,Chef,Administrador")]
/*PROTECTED REGION END*/
public partial class ListaCompraController : Controller
{
    /*PROTECTED REGION ID(ListaCompraController_dependencies) START*/
    private readonly ListaCompraService _listaCompraService;
    private readonly ILogger<ListaCompraController> _logger;
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public ListaCompraController(ListaCompraService listaCompraService, Edible.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<ListaCompraController>? logger = null)
    {
        _listaCompraService = listaCompraService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<ListaCompraController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(ListaCompraController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(ListaCompraController_Index_GET_body) START*/

        var entities = await _listaCompraService.ReadAllAsync();
        return View(entities.Select(ListaCompraAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ListaCompraController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(ListaCompraController_Details_GET_body) START*/

        var entity = await _listaCompraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(ListaCompraAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ListaCompraController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(ListaCompraController_Create_GET_body) START*/

        var model = new Edible.Web.ViewModels.ListaCompraViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ListaCompraController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(Edible.Web.ViewModels.ListaCompraViewModel model)
    {
    /*PROTECTED REGION ID(ListaCompraController_Create_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.ListaCompraService(unitOfWork);



        model.IngredienteEnLaLista ??= new();
        if (model.IngredienteEnLaLista.Count > 500) return BadRequest("Número de líneas no válido (máximo 500).");
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
        if (row.NeveraId is int selectedRowIngredienteEnLaListaNevera &&
            !row.NeveraOptions.Any(option => option.Value == Convert.ToString(selectedRowIngredienteEnLaListaNevera, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"IngredienteEnLaLista[{index}].NeveraId", "Selecciona un registro existente de Nevera.");
        if (row.NeveraId is null) ModelState.AddModelError($"IngredienteEnLaLista[{index}].NeveraId", "Debes seleccionar una opción.");
        if (row.IngredienteCatalogoId is int selectedRowIngredienteEnLaListaIngredienteCatalogo &&
            !row.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedRowIngredienteEnLaListaIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"IngredienteEnLaLista[{index}].IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (row.IngredienteCatalogoId is null) ModelState.AddModelError($"IngredienteEnLaLista[{index}].IngredienteCatalogoId", "Debes seleccionar una opción.");
        }


        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.ListaCompra.Create");
            var submittedIngredienteEnLaLista = await service.PrepareListaCompraIngredienteEnLaListaAsync(null,
                model.IngredienteEnLaLista.Select(row => (row.Id, IngredienteCompraAssembler.Create(row), row.IngredienteCatalogoId, row.NeveraId)).ToList());

            var entity = await service.NewAsync(model.UsuarioId.GetValueOrDefault());

            await service.ApplyListaCompraRelationsAsync(entity, p_usuario_id: model.UsuarioId, p_ingredienteEnLaLista: submittedIngredienteEnLaLista);

            await service.ModifierListaCompraDefaultAsync(entity);


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
    /*PROTECTED REGION ID(ListaCompraController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(ListaCompraController_Edit_GET_body) START*/

        var entity = await _listaCompraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = ListaCompraAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ListaCompraController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, Edible.Web.ViewModels.ListaCompraViewModel model)
    {
    /*PROTECTED REGION ID(ListaCompraController_Edit_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.ListaCompraService(unitOfWork);

        var entity = await service.ReadOIDListaCompraDefaultAsync(id);
        if (entity is null) return NotFound();

        model.IngredienteEnLaLista ??= new();
        if (model.IngredienteEnLaLista.Count > 500) return BadRequest("Número de líneas no válido (máximo 500).");
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
        if (row.NeveraId is int selectedRowIngredienteEnLaListaNevera &&
            !row.NeveraOptions.Any(option => option.Value == Convert.ToString(selectedRowIngredienteEnLaListaNevera, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"IngredienteEnLaLista[{index}].NeveraId", "Selecciona un registro existente de Nevera.");
        if (row.NeveraId is null) ModelState.AddModelError($"IngredienteEnLaLista[{index}].NeveraId", "Debes seleccionar una opción.");
        if (row.IngredienteCatalogoId is int selectedRowIngredienteEnLaListaIngredienteCatalogo &&
            !row.IngredienteCatalogoOptions.Any(option => option.Value == Convert.ToString(selectedRowIngredienteEnLaListaIngredienteCatalogo, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError($"IngredienteEnLaLista[{index}].IngredienteCatalogoId", "Selecciona un registro existente de Ingrediente.");
        if (row.IngredienteCatalogoId is null) ModelState.AddModelError($"IngredienteEnLaLista[{index}].IngredienteCatalogoId", "Debes seleccionar una opción.");
        }


        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.ListaCompra.Edit");
            var submittedIngredienteEnLaLista = await service.PrepareListaCompraIngredienteEnLaListaAsync(entity,
                model.IngredienteEnLaLista.Select(row => (row.Id, IngredienteCompraAssembler.Create(row), row.IngredienteCatalogoId, row.NeveraId)).ToList());

            await service.ModifyAsync(id);
            entity = await service.ReadOIDListaCompraDefaultAsync(id) ?? throw new InvalidOperationException("Entity disappeared during update.");
            await service.ApplyListaCompraRelationsAsync(entity, p_usuario_id: model.UsuarioId, p_ingredienteEnLaLista: submittedIngredienteEnLaLista);

            await service.ModifierListaCompraDefaultAsync(entity);


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
    /*PROTECTED REGION ID(ListaCompraController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(ListaCompraController_Delete_GET_body) START*/

        var entity = await _listaCompraService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(ListaCompraAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ListaCompraController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(ListaCompraController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new Edible.Web.ViewModels.ListaCompraViewModel());
        await _listaCompraService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(ListaCompraController_method_LoadFormOptionsAsync_EdibleWebViewModelsListaCompraViewModel) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.ListaCompraViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ListaCompraController_method_LoadFormOptionsAsync_EdibleWebViewModelsListaCompraViewModel_EdibleApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.ListaCompraViewModel model, Edible.Application.Abstractions.IUnitOfWork unitOfWork)
    {
        if (model.IngredienteEnLaLista.Count > 500) { ModelState.AddModelError(nameof(model.IngredienteEnLaLista), "Máximo 500 líneas."); return; }
        var options0 = (await unitOfWork.UsuarioRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Name, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options1 = (await unitOfWork.NeveraRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options2 = (await unitOfWork.IngredienteRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Nombre, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        model.UsuarioOptions = options0.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        foreach (var row in model.IngredienteEnLaLista)
        {
        row.NeveraOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
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
        model.IngredienteEnLaListaTemplate.NeveraOptions = options1.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
        model.IngredienteEnLaListaTemplate.IngredienteCatalogoOptions = options2.Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = item.Value,
            Text = item.Text
        }).ToList();
    }
    /*PROTECTED REGION END*/

}
