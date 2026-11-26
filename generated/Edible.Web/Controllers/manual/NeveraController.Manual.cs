// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(NeveraController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Application.Exceptions;
using Edible.Web.Assemblers;
using Edible.Web.ViewModels;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION ID(NeveraController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// La autorización de clase ([Authorize(Roles = "Usuario,Chef,Administrador")]) está en NeveraController.cs.
/*PROTECTED REGION END*/
public partial class NeveraController
{
    private const string RolAdministrador = "Administrador";

    private bool IsAdministrador => User.IsInRole(RolAdministrador);

    private int? CurrentUserId =>
        int.TryParse(User?.Identity?.Name, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture, out var id) ? id : null;

    // ------------------------------------------------------------------
    // RecorrerListaCompra (CustomTransaction)
    // ------------------------------------------------------------------

    [HttpGet]
    public async Task<IActionResult> RecorrerListaCompra(int? usuarioId = null)
    {
        var model = new RecorrerListaCompraViewModel
        {
            UsuarioId = IsAdministrador ? (usuarioId ?? CurrentUserId) : CurrentUserId
        };
        await LoadRecorrerListaOptionsAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(NeveraController_RecorrerListaCompra_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> RecorrerListaCompra(int p_usuario_oid)
    {
    /*PROTECTED REGION ID(NeveraController_RecorrerListaCompra_POST_body) ENABLED START*/

        if (!IsAdministrador)
        {
            if (CurrentUserId is not int currentId) return Challenge();
            if (p_usuario_oid > 0 && p_usuario_oid != currentId) return Forbid();
            p_usuario_oid = currentId;
        }

        if (!ModelState.IsValid || p_usuario_oid <= 0)
        {
            TempData["Error"] = "Debes seleccionar un usuario válido para volcar su lista de la compra.";
            return RedirectToAction(nameof(RecorrerListaCompra));
        }

        try
        {
            await using var unitOfWork = _useCaseUnitOfWorkFactory.Create();
            var useCase = new Edible.Application.UseCases.Nevera.NeveraUseCase(unitOfWork);
            var nevera = await useCase.RecorrerListaCompraAsync(p_usuario_oid);
            TempData["Success"] = IsAdministrador
                ? $"Se han volcado los ingredientes comprados a la nevera #{nevera.Id} y se han eliminado de la lista de la compra."
                : "Se han volcado los productos comprados a tu nevera y se han quitado de tu lista de la compra.";
        }
        catch (ModelException error)
        {
            TempData["Error"] = error.InnerException?.Message ?? error.Message;
        }

        return RedirectToAction(nameof(RecorrerListaCompra), IsAdministrador ? new { usuarioId = p_usuario_oid } : null);
    /*PROTECTED REGION END*/
    }

    private async Task LoadRecorrerListaOptionsAsync(RecorrerListaCompraViewModel model)
    {
        await using var unitOfWork = _formUnitOfWorkFactory.Create();
        var usuarios = (await unitOfWork.UsuarioRepository.ReadAllAsync())
            .OrderBy(u => u.Name)
            .ToList();

        if (!IsAdministrador && CurrentUserId is int uid)
        {
            usuarios = usuarios.Where(u => u.Id == uid).ToList();
            model.UsuarioId = uid;
        }
        else if (model.UsuarioId is null || !usuarios.Any(u => u.Id == model.UsuarioId))
        {
            model.UsuarioId = usuarios.FirstOrDefault()?.Id;
        }

        model.UsuarioOptions = usuarios
            .Select(u => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = u.Id.ToString(System.Globalization.CultureInfo.InvariantCulture),
                Text = $"{u.Name} (#{u.Id})"
            }).ToList();

        if (model.UsuarioId is int selectedUserId)
        {
            var usuario = usuarios.FirstOrDefault(u => u.Id == selectedUserId);
            model.UsuarioNombre = usuario?.Name;
            var neveraId = usuario?.Nevera?.Id;
            var listaId = usuario?.ListaCompra?.Id;

            if (listaId is int lid)
            {
                var itemsCompra = await unitOfWork.IngredienteCompraRepository.ReadAllAsync();
                model.ItemsListaCompra = itemsCompra
                    .Where(i => i.ListaCompra?.Id == lid)
                    .OrderByDescending(i => i.Comprado)
                    .ThenBy(i => i.IngredienteCatalogo?.Nombre)
                    .Select(i => new RecorrerListaCompraViewModel.ItemCompraResumen(
                        i.Id,
                        i.IngredienteCatalogo?.Nombre ?? $"#{i.Id}",
                        i.Cantidad,
                        i.Comprado))
                    .ToList();
            }

            if (neveraId is int nid)
            {
                var itemsNevera = await unitOfWork.IngredienteNeveraRepository.ReadAllAsync();
                model.ItemsNevera = itemsNevera
                    .Where(i => i.Nevera?.Id == nid)
                    .OrderBy(i => i.FechaCaducidad)
                    .Select(i => new RecorrerListaCompraViewModel.ItemNeveraResumen(
                        i.Id,
                        i.IngredienteCatalogo?.Nombre ?? $"#{i.Id}",
                        i.Cantidad,
                        i.FechaCaducidad))
                    .ToList();
            }
        }
    }
}
