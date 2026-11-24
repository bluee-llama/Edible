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
[Authorize(Roles = "Usuario,Chef,Administrador")]
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
    /*PROTECTED REGION ID(UsuarioController_Index_GET_attributes) ENABLED START*/
    [Authorize(Roles = "Administrador")]
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
    /*PROTECTED REGION ID(UsuarioController_Details_GET_body) ENABLED START*/

        // Un usuario solo puede ver su propio perfil; el Administrador puede ver cualquiera.
        if (!User.CanManageUsuario(id)) return Forbid();

        var entity = await _usuarioService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(UsuarioAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Create_GET_attributes) ENABLED START*/
    [Authorize(Roles = "Administrador")]
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
    /*PROTECTED REGION ID(UsuarioController_Create_POST_attributes) ENABLED START*/
    [Authorize(Roles = "Administrador")]
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(Edible.Web.ViewModels.UsuarioViewModel model)
    {
    /*PROTECTED REGION ID(UsuarioController_Create_POST_body) ENABLED START*/

        // La Nevera y la ListaCompra se crean automáticamente junto con el Usuario:
        // no se eligen en el formulario, así que se ignora su validación [Required].
        ModelState.Remove(nameof(model.NeveraId));
        ModelState.Remove(nameof(model.ListaCompraId));

        if (string.IsNullOrEmpty(model.Password)) ModelState.AddModelError(nameof(model.Password), "Debes indicar una contraseña.");
        // El nombre del administrador está reservado: evita que alguien se registre como "admin".
        if (HariaWebAuthentication.IsReservedAdministratorName(model.Name))
            ModelState.AddModelError(nameof(model.Name), "Ese nombre está reservado. Elige otro.");

        if (!ModelState.IsValid) return View(model);

        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.UsuarioService(unitOfWork);

        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Usuario.Create");

            await service.NewAsync(model.Name, model.Password!, model.Publico, model.Dieta, new Nevera(), new ListaCompra());

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

        TempData["Success"] = "Usuario creado correctamente (con su Nevera y su Lista de la compra).";
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
    /*PROTECTED REGION ID(UsuarioController_Register_POST_body) ENABLED START*/

        // La Nevera y la ListaCompra se crean automáticamente junto con el Usuario:
        // no se eligen en el formulario, así que se ignora su validación [Required].
        ModelState.Remove(nameof(model.NeveraId));
        ModelState.Remove(nameof(model.ListaCompraId));

        if (string.IsNullOrEmpty(model.Password)) ModelState.AddModelError(nameof(model.Password), "Debes indicar una contraseña.");
        // El nombre del administrador está reservado: evita que alguien se registre como "admin".
        if (HariaWebAuthentication.IsReservedAdministratorName(model.Name))
            ModelState.AddModelError(nameof(model.Name), "Ese nombre está reservado. Elige otro.");

        if (!ModelState.IsValid) return View(model);

        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.UsuarioService(unitOfWork);

        Usuario createdUser;
        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Usuario.Create");

            var entity = await service.NewAsync(model.Name, model.Password!, model.Publico, model.Dieta, new Nevera(), new ListaCompra());
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
        TempData["Success"] = $"Tu cuenta se ha creado correctamente. Tu identificador para iniciar sesión es {createdUser.Id}: guárdalo.";
        return RedirectToAction("Index", "Home");
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(UsuarioController_Edit_GET_body) ENABLED START*/

        // Un usuario solo puede editar su propio perfil; el Administrador puede editar cualquiera.
        if (!User.CanManageUsuario(id)) return Forbid();

        var entity = await _usuarioService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = UsuarioAssembler.ToViewModel(entity);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(UsuarioController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, Edible.Web.ViewModels.UsuarioViewModel model)
    {
    /*PROTECTED REGION ID(UsuarioController_Edit_POST_body) ENABLED START*/

        // Un usuario solo puede editar su propio perfil; el Administrador puede editar cualquiera.
        if (!User.CanManageUsuario(id)) return Forbid();

        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.UsuarioService(unitOfWork);

        var entity = await service.ReadOIDUsuarioDefaultAsync(id);
        if (entity is null) return NotFound();

        // La Nevera y la ListaCompra no son editables: se conservan las del servidor
        // (nunca se confía en los ids recibidos del formulario).
        ModelState.Remove(nameof(model.NeveraId));
        ModelState.Remove(nameof(model.ListaCompraId));
        model.Id = id;
        model.NeveraId = entity.Nevera?.Id;
        model.ListaCompraId = entity.ListaCompra?.Id;
        model.NeveraLabel = entity.Nevera?.Id.ToString();
        model.ListaCompraLabel = entity.ListaCompra?.Id.ToString();

        // No se puede tomar el nombre reservado del administrador (salvo que ya sea el suyo).
        if (HariaWebAuthentication.IsReservedAdministratorName(model.Name) &&
            !string.Equals(entity.Name?.Trim(), model.Name?.Trim(), StringComparison.OrdinalIgnoreCase))
            ModelState.AddModelError(nameof(model.Name), "Ese nombre está reservado. Elige otro.");

        if (!ModelState.IsValid) return View(model);

        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Usuario.Edit");

            await service.ModifyAsync(id, model.Name, (string.IsNullOrEmpty(model.Password) ? entity.Password : model.Password), model.Publico, model.Dieta, entity.Nevera, entity.ListaCompra);

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

        TempData["Success"] = "Cambios guardados correctamente.";
        return User.IsAdministrador()
            ? RedirectToAction(nameof(Index))
            : RedirectToAction(nameof(Details), new { id });
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(UsuarioController_Delete_GET_attributes) ENABLED START*/
    [Authorize(Roles = "Administrador")]
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
    /*PROTECTED REGION ID(UsuarioController_DeleteConfirmed_POST_attributes) ENABLED START*/
    [Authorize(Roles = "Administrador")]
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
