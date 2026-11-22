// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(UsuarioController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;
using Edible.Web.Utils.Security;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

/*PROTECTED REGION ID(UsuarioController_class_attributes) ENABLED START*/

/*PROTECTED REGION END*/
public partial class UsuarioController : Controller
{
    /*PROTECTED REGION ID(UsuarioController_dependencies) START*/
    private readonly UsuarioService _usuarioService;
    private readonly ILogger<UsuarioController> _logger;
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public UsuarioController(UsuarioService usuarioService, Edible.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<UsuarioController>? logger = null)
    {
        _usuarioService = usuarioService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<UsuarioController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Index_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(UsuarioController_Index_GET_body) START*/

        var entities = await _usuarioService.ReadAllAsync();
        return View(entities.Select(UsuarioAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(UsuarioController_Details_GET_body) START*/

        var entity = await _usuarioService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(UsuarioAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Create_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(UsuarioController_Create_GET_body) START*/

        var model = new Edible.Web.ViewModels.UsuarioViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(UsuarioController_Create_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(Edible.Web.ViewModels.UsuarioViewModel model)
    {
    /*PROTECTED REGION ID(UsuarioController_Create_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.UsuarioService(unitOfWork);





        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.NeveraId is int selectedNevera &&
            !model.NeveraOptions.Any(option => option.Value == Convert.ToString(selectedNevera, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("NeveraId", "Selecciona un registro existente de Nevera.");
        if (model.NeveraId is null) ModelState.AddModelError("NeveraId", "Debes seleccionar una opción.");
        if (model.ListaCompraId is int selectedListaCompra &&
            !model.ListaCompraOptions.Any(option => option.Value == Convert.ToString(selectedListaCompra, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ListaCompraId", "Selecciona un registro existente de ListaCompra.");
        if (model.ListaCompraId is null) ModelState.AddModelError("ListaCompraId", "Debes seleccionar una opción.");


        if (string.IsNullOrEmpty(model.Password)) ModelState.AddModelError(nameof(model.Password), "Debes indicar una contraseña.");

        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Usuario.Create");

            var argumentNevera = await unitOfWork.NeveraRepository.ReadAsync(model.NeveraId.GetValueOrDefault()) ?? throw new InvalidOperationException("Nevera not found.");
            var argumentListaCompra = await unitOfWork.ListaCompraRepository.ReadAsync(model.ListaCompraId.GetValueOrDefault()) ?? throw new InvalidOperationException("ListaCompra not found.");
            var entity = await service.NewAsync(model.Name, model.Password!, model.Publico, model.Dieta, argumentNevera, argumentListaCompra);

            await service.ApplyUsuarioRelationsAsync(entity, p_nevera_id: model.NeveraId, p_listaCompra_id: model.ListaCompraId);

            await service.ModifierUsuarioDefaultAsync(entity);


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
    /*PROTECTED REGION ID(UsuarioController_Register_GET_attributes) START*/
    [AllowAnonymous]
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Register()
    {
    /*PROTECTED REGION ID(UsuarioController_Register_GET_body) START*/

        var model = new Edible.Web.ViewModels.UsuarioViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(UsuarioController_Register_POST_attributes) START*/
    [AllowAnonymous]
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Register(Edible.Web.ViewModels.UsuarioViewModel model)
    {
    /*PROTECTED REGION ID(UsuarioController_Register_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.UsuarioService(unitOfWork);





        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.NeveraId is int selectedNevera &&
            !model.NeveraOptions.Any(option => option.Value == Convert.ToString(selectedNevera, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("NeveraId", "Selecciona un registro existente de Nevera.");
        if (model.NeveraId is null) ModelState.AddModelError("NeveraId", "Debes seleccionar una opción.");
        if (model.ListaCompraId is int selectedListaCompra &&
            !model.ListaCompraOptions.Any(option => option.Value == Convert.ToString(selectedListaCompra, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ListaCompraId", "Selecciona un registro existente de ListaCompra.");
        if (model.ListaCompraId is null) ModelState.AddModelError("ListaCompraId", "Debes seleccionar una opción.");


        if (string.IsNullOrEmpty(model.Password)) ModelState.AddModelError(nameof(model.Password), "Debes indicar una contraseña.");

        if (!ModelState.IsValid) return View(model);

        Usuario createdUser;
        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Usuario.Create");

            var argumentNevera = await unitOfWork.NeveraRepository.ReadAsync(model.NeveraId.GetValueOrDefault()) ?? throw new InvalidOperationException("Nevera not found.");
            var argumentListaCompra = await unitOfWork.ListaCompraRepository.ReadAsync(model.ListaCompraId.GetValueOrDefault()) ?? throw new InvalidOperationException("ListaCompra not found.");
            var entity = await service.NewAsync(model.Name, model.Password!, model.Publico, model.Dieta, argumentNevera, argumentListaCompra);

            await service.ApplyUsuarioRelationsAsync(entity, p_nevera_id: model.NeveraId, p_listaCompra_id: model.ListaCompraId);

            await service.ModifierUsuarioDefaultAsync(entity);
            createdUser = entity;

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

        try
        {
            await HariaWebAuthentication.SignInAsync(HttpContext, "Usuario", createdUser.Id, createdUser);
            HttpContext.Session.SetUser(UsuarioAssembler.ToViewModel(createdUser));
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            _logger.LogError(error, "La cuenta se ha creado, pero no se ha podido iniciar la sesión.");
            ModelState.AddModelError(string.Empty, "Tu cuenta ya está creada. Inicia sesión desde la página de acceso.");
            return View(model);
        }
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(UsuarioController_Edit_GET_body) START*/

        var entity = await _usuarioService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = UsuarioAssembler.ToViewModel(entity);
        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(UsuarioController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, Edible.Web.ViewModels.UsuarioViewModel model)
    {
    /*PROTECTED REGION ID(UsuarioController_Edit_POST_body) START*/


        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.UsuarioService(unitOfWork);

        var entity = await service.ReadOIDUsuarioDefaultAsync(id);
        if (entity is null) return NotFound();



        await LoadFormOptionsAsync(model, unitOfWork);
        if (model.NeveraId is int selectedNevera &&
            !model.NeveraOptions.Any(option => option.Value == Convert.ToString(selectedNevera, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("NeveraId", "Selecciona un registro existente de Nevera.");
        if (model.NeveraId is null) ModelState.AddModelError("NeveraId", "Debes seleccionar una opción.");
        if (model.ListaCompraId is int selectedListaCompra &&
            !model.ListaCompraOptions.Any(option => option.Value == Convert.ToString(selectedListaCompra, System.Globalization.CultureInfo.InvariantCulture)))
            ModelState.AddModelError("ListaCompraId", "Selecciona un registro existente de ListaCompra.");
        if (model.ListaCompraId is null) ModelState.AddModelError("ListaCompraId", "Debes seleccionar una opción.");




        if (!ModelState.IsValid) return View(model);


        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Usuario.Edit");

            var argumentNevera = await unitOfWork.NeveraRepository.ReadAsync(model.NeveraId.GetValueOrDefault()) ?? throw new InvalidOperationException("Nevera not found.");
            var argumentListaCompra = await unitOfWork.ListaCompraRepository.ReadAsync(model.ListaCompraId.GetValueOrDefault()) ?? throw new InvalidOperationException("ListaCompra not found.");
            await service.ModifyAsync(id, model.Name, (string.IsNullOrEmpty(model.Password) ? entity.Password : model.Password), model.Publico, model.Dieta, argumentNevera, argumentListaCompra);
            entity = await service.ReadOIDUsuarioDefaultAsync(id) ?? throw new InvalidOperationException("Entity disappeared during update.");
            await service.ApplyUsuarioRelationsAsync(entity, p_nevera_id: model.NeveraId, p_listaCompra_id: model.ListaCompraId);

            await service.ModifierUsuarioDefaultAsync(entity);


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
    /*PROTECTED REGION ID(UsuarioController_Delete_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(UsuarioController_Delete_GET_body) START*/

        var entity = await _usuarioService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(UsuarioAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(UsuarioController_DeleteConfirmed_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(UsuarioController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new Edible.Web.ViewModels.UsuarioViewModel());
        await _usuarioService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(UsuarioController_method_LoadFormOptionsAsync_EdibleWebViewModelsUsuarioViewModel) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.UsuarioViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(UsuarioController_method_LoadFormOptionsAsync_EdibleWebViewModelsUsuarioViewModel_EdibleApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.UsuarioViewModel model, Edible.Application.Abstractions.IUnitOfWork unitOfWork)
    {

        var options0 = (await unitOfWork.NeveraRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
        var options1 = (await unitOfWork.ListaCompraRepository.ReadAllAsync()).Select(item => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem { Value = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture), Text = Convert.ToString(item.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty }).ToList();
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

    }
    /*PROTECTED REGION END*/

}
