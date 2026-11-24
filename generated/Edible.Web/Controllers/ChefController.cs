// HARIA-WEB MVC managed controller
/*PROTECTED REGION ID(ChefController_usings) START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Web.Assemblers;
using Edible.Web.Utils.Security;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

/*PROTECTED REGION ID(ChefController_class_attributes) ENABLED START*/
[Authorize(Roles = "Chef,Administrador")]
/*PROTECTED REGION END*/
public partial class ChefController : Controller
{
    /*PROTECTED REGION ID(ChefController_dependencies) START*/
    private readonly ChefService _chefService;
    private readonly UsuarioService _usuarioService;
    private readonly ILogger<ChefController> _logger;
    private readonly Edible.Application.Abstractions.IUnitOfWorkFactory _formUnitOfWorkFactory;
    public ChefController(ChefService chefService, UsuarioService usuarioService, Edible.Application.Abstractions.IUnitOfWorkFactory formUnitOfWorkFactory, ILogger<ChefController>? logger = null)
    {
        _chefService = chefService;
        _usuarioService = usuarioService;

        _formUnitOfWorkFactory = formUnitOfWorkFactory;

        _logger = logger ?? global::Microsoft.Extensions.Logging.Abstractions.NullLogger<ChefController>.Instance;
    }
    /*PROTECTED REGION END*/
    [HttpGet]
    /*PROTECTED REGION ID(ChefController_Index_GET_attributes) ENABLED START*/
    [Authorize(Roles = "Administrador")]
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Index()
    {
    /*PROTECTED REGION ID(ChefController_Index_GET_body) START*/

        var entities = await _chefService.ReadAllAsync();
        return View(entities.Select(ChefAssembler.ToViewModel).ToList());
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ChefController_Details_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Details(int id)
    {
    /*PROTECTED REGION ID(ChefController_Details_GET_body) ENABLED START*/

        // Un usuario solo puede ver su propio perfil; el Administrador puede ver cualquiera.
        if (!User.CanManageUsuario(id)) return Forbid();

        var entity = await _chefService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(ChefAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ChefController_Create_GET_attributes) ENABLED START*/
    [Authorize(Roles = "Administrador")]
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create()
    {
    /*PROTECTED REGION ID(ChefController_Create_GET_body) START*/

        var model = new Edible.Web.ViewModels.ChefViewModel();

        await LoadFormOptionsAsync(model);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ChefController_Create_POST_attributes) ENABLED START*/
    [Authorize(Roles = "Administrador")]
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Create(Edible.Web.ViewModels.ChefViewModel model)
    {
    /*PROTECTED REGION ID(ChefController_Create_POST_body) ENABLED START*/

        // La Nevera y la ListaCompra se crean automáticamente junto con el Chef:
        // no se eligen en el formulario, así que se ignora su validación [Required].
        ModelState.Remove(nameof(model.NeveraId));
        ModelState.Remove(nameof(model.ListaCompraId));

        if (string.IsNullOrEmpty(model.Password)) ModelState.AddModelError(nameof(model.Password), "Debes indicar una contraseña.");
        // El nombre del administrador está reservado: evita que alguien se registre como "admin".
        if (HariaWebAuthentication.IsReservedAdministratorName(model.Name))
            ModelState.AddModelError(nameof(model.Name), "Ese nombre está reservado. Elige otro.");

        if (!ModelState.IsValid) return View(model);

        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.ChefService(unitOfWork);

        try
        {
            await unitOfWork.BeginTransactionAsync("MVC.Chef.Create");

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

        TempData["Success"] = "Chef creado correctamente (con su Nevera y su Lista de la compra).";
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpGet]
    /*PROTECTED REGION ID(ChefController_Edit_GET_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id)
    {
    /*PROTECTED REGION ID(ChefController_Edit_GET_body) ENABLED START*/

        // Un usuario solo puede editar su propio perfil; el Administrador puede editar cualquiera.
        if (!User.CanManageUsuario(id)) return Forbid();

        var entity = await _chefService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        var model = ChefAssembler.ToViewModel(entity);
        return View(model);
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ChefController_Edit_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Edit(int id, Edible.Web.ViewModels.ChefViewModel model)
    {
    /*PROTECTED REGION ID(ChefController_Edit_POST_body) ENABLED START*/

        // Un usuario solo puede editar su propio perfil; el Administrador puede editar cualquiera.
        if (!User.CanManageUsuario(id)) return Forbid();

        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var service = new global::Edible.Application.Services.ChefService(unitOfWork);

        var entity = await service.ReadOIDChefDefaultAsync(id);
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
            await unitOfWork.BeginTransactionAsync("MVC.Chef.Edit");

            await service.ModifyAsync(id, model.Name, (string.IsNullOrEmpty(model.Password) ? entity.Password : model.Password), model.Publico, model.Dieta);

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
    /*PROTECTED REGION ID(ChefController_Delete_GET_attributes) ENABLED START*/
    [Authorize(Roles = "Administrador")]
    /*PROTECTED REGION END*/
    public async Task<IActionResult> Delete(int id)
    {
    /*PROTECTED REGION ID(ChefController_Delete_GET_body) START*/

        var entity = await _chefService.ReadOIDAsync(id);
        if (entity is null) return NotFound();
        return View(ChefAssembler.ToViewModel(entity));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(ChefController_DeleteConfirmed_POST_attributes) ENABLED START*/
    [Authorize(Roles = "Administrador")]
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
    /*PROTECTED REGION ID(ChefController_DeleteConfirmed_POST_body) START*/

        if (!ModelState.IsValid) return View("Delete", new Edible.Web.ViewModels.ChefViewModel());
        await _chefService.DestroyAsync(id);
        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    /*PROTECTED REGION ID(ChefController_method_LoadFormOptionsAsync_EdibleWebViewModelsChefViewModel) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.ChefViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        await LoadFormOptionsAsync(model, unitOfWork);
    }
    /*PROTECTED REGION END*/

    /*PROTECTED REGION ID(ChefController_method_LoadFormOptionsAsync_EdibleWebViewModelsChefViewModel_EdibleApplicationAbstractionsIUnitOfWork) START*/
    private async Task LoadFormOptionsAsync(Edible.Web.ViewModels.ChefViewModel model, Edible.Application.Abstractions.IUnitOfWork unitOfWork)
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
