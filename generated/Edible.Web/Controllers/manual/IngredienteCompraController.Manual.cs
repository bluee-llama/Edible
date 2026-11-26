// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(IngredienteCompraController_manual_usings) ENABLED START*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Edible.Domain.Entities;
using Edible.Application.Services;
using Edible.Application.Exceptions;
using Edible.Web.Assemblers;

using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Web.Controllers;

// Class-level authorization applies to both partial files.
// [Authorize(Roles = "Admin")]
/*PROTECTED REGION ID(IngredienteCompraController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// La autorización de clase ([Authorize(Roles = "Usuario,Chef,Administrador")]) está en IngredienteCompraController.cs.
/*PROTECTED REGION END*/
public partial class IngredienteCompraController
{
    private const string RolAdministrador = "Administrador";

    private bool IsAdministrador => User.IsInRole(RolAdministrador);

    private int? CurrentUserId =>
        int.TryParse(User?.Identity?.Name, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture, out var id) ? id : null;

    private async Task<(int? NeveraId, int? ListaCompraId)> GetCurrentUserNeveraAndListaAsync(
        Edible.Application.Abstractions.IUnitOfWork? existingUow = null)
    {
        if (CurrentUserId is not int uid) return (null, null);
        if (existingUow is not null)
        {
            var neveras = await existingUow.NeveraRepository.ReadAllAsync();
            var listas = await existingUow.ListaCompraRepository.ReadAllAsync();
            return (
                neveras.FirstOrDefault(n => n.Usuario?.Id == uid)?.Id,
                listas.FirstOrDefault(l => l.Usuario?.Id == uid)?.Id);
        }

        await using var uow = _formUnitOfWorkFactory.Create();
        var todasNeveras = await uow.NeveraRepository.ReadAllAsync();
        var todasListas = await uow.ListaCompraRepository.ReadAllAsync();
        return (
            todasNeveras.FirstOrDefault(n => n.Usuario?.Id == uid)?.Id,
            todasListas.FirstOrDefault(l => l.Usuario?.Id == uid)?.Id);
    }

    private async Task<bool> CanAccessCompraItemAsync(
        IngredienteCompra entity,
        Edible.Application.Abstractions.IUnitOfWork? existingUow = null)
    {
        if (IsAdministrador) return true;
        if (entity.ListaCompra?.Usuario?.Id is int ownerId) return ownerId == CurrentUserId;
        var (_, myListaId) = await GetCurrentUserNeveraAndListaAsync(existingUow);
        return myListaId is not null && entity.ListaCompra?.Id == myListaId;
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteCompraController_MarcarComprado_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> MarcarComprado(int p_oid)
    {
    /*PROTECTED REGION ID(IngredienteCompraController_MarcarComprado_POST_body) ENABLED START*/

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Identificador de ingrediente de compra no válido.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var entity = await _ingredienteCompraService.ReadOIDAsync(p_oid);
            if (entity is null) return NotFound();
            if (!await CanAccessCompraItemAsync(entity)) return Forbid();

            await _ingredienteCompraService.MarcarCompradoAsync(p_oid);
            TempData["Success"] = "Producto marcado como comprado.";
        }
        catch (ModelException error)
        {
            TempData["Error"] = error.InnerException?.Message ?? error.Message;
        }

        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteCompraController_DesmarcarComprado_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> DesmarcarComprado(int p_oid)
    {
    /*PROTECTED REGION ID(IngredienteCompraController_DesmarcarComprado_POST_body) ENABLED START*/

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Identificador de ingrediente de compra no válido.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var entity = await _ingredienteCompraService.ReadOIDAsync(p_oid);
            if (entity is null) return NotFound();
            if (!await CanAccessCompraItemAsync(entity)) return Forbid();

            await _ingredienteCompraService.DesmarcarCompradoAsync(p_oid);
            TempData["Success"] = "Producto desmarcado como pendiente de compra.";
        }
        catch (ModelException error)
        {
            TempData["Error"] = error.InnerException?.Message ?? error.Message;
        }

        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }
}
