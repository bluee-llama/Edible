// HARIA-WEB MVC managed manual-controller
/*PROTECTED REGION ID(IngredienteNeveraController_manual_usings) ENABLED START*/
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
/*PROTECTED REGION ID(IngredienteNeveraController_manual_class_attributes) ENABLED START*/
// Class-level authorization applies to both partial files.
// La autorización de clase ([Authorize(Roles = "Usuario,Chef,Administrador")]) está en IngredienteNeveraController.cs.
/*PROTECTED REGION END*/
public partial class IngredienteNeveraController
{
    private const string RolAdministrador = "Administrador";

    private bool IsAdministrador => User.IsInRole(RolAdministrador);

    private int? CurrentUserId =>
        int.TryParse(User?.Identity?.Name, System.Globalization.NumberStyles.Integer,
            System.Globalization.CultureInfo.InvariantCulture, out var id) ? id : null;

    private async Task<int?> GetCurrentUserNeveraIdAsync(Edible.Application.Abstractions.IUnitOfWork? existingUow = null)
    {
        if (CurrentUserId is not int uid) return null;
        if (existingUow is not null)
        {
            var neveras = await existingUow.NeveraRepository.ReadAllAsync();
            return neveras.FirstOrDefault(n => n.Usuario?.Id == uid)?.Id;
        }

        await using var uow = _formUnitOfWorkFactory.Create();
        var todas = await uow.NeveraRepository.ReadAllAsync();
        return todas.FirstOrDefault(n => n.Usuario?.Id == uid)?.Id;
    }

    private async Task<bool> CanAccessNeveraItemAsync(IngredienteNevera entity, Edible.Application.Abstractions.IUnitOfWork? existingUow = null)
    {
        if (IsAdministrador) return true;
        if (entity.Nevera?.Usuario?.Id is int ownerId) return ownerId == CurrentUserId;
        var myNeveraId = await GetCurrentUserNeveraIdAsync(existingUow);
        return myNeveraId is not null && entity.Nevera?.Id == myNeveraId;
    }

    // ------------------------------------------------------------------
    // FiltrarNevera (ReadFilter) — Construcción del ViewModel para la vista HTML
    // ------------------------------------------------------------------

    private async Task<FiltrarNeveraViewModel> BuildFiltrarNeveraAsync(
        int p_nevera_oid, TipoIngrediente p_tipo, DateTime p_fechaLimiteCerca, DateTime p_fechaActual, EstadoCaducidad p_estado)
    {
        var buscado = Request?.Query.Count > 0;
        var fechaActual = p_fechaActual == default ? DateTime.Today : p_fechaActual.Date;
        var fechaLimiteCerca = p_fechaLimiteCerca == default ? fechaActual.AddDays(3) : p_fechaLimiteCerca.Date;

        var model = new FiltrarNeveraViewModel
        {
            NeveraId = p_nevera_oid,
            Tipo = p_tipo,
            FechaActual = fechaActual,
            FechaLimiteCerca = fechaLimiteCerca,
            Estado = p_estado,
            Buscado = buscado
        };

        await using (var unitOfWork = _formUnitOfWorkFactory.Create())
        {
            var neveras = (await unitOfWork.NeveraRepository.ReadAllAsync()).ToList();
            if (!IsAdministrador)
            {
                var myNeveraId = neveras.FirstOrDefault(n => CurrentUserId is int uid && n.Usuario?.Id == uid)?.Id ?? 0;
                p_nevera_oid = myNeveraId;
                model.NeveraId = myNeveraId;
                neveras = neveras.Where(n => n.Id == myNeveraId).ToList();
            }
            else if (model.NeveraId <= 0)
            {
                model.NeveraId = neveras.FirstOrDefault(n => CurrentUserId is int uid && n.Usuario?.Id == uid)?.Id
                    ?? neveras.FirstOrDefault()?.Id
                    ?? 0;
            }

            model.NeveraOptions = neveras
                .OrderBy(n => n.Id)
                .Select(n => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = Convert.ToString(n.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty,
                    Text = n.Usuario is not null ? $"Nevera #{n.Id} ({n.Usuario.Name})" : $"Nevera #{n.Id}"
                }).ToList();
        }

        if (model.Buscado)
        {
            if (p_nevera_oid <= 0)
            {
                ModelState.AddModelError(nameof(model.NeveraId), "Debes seleccionar una nevera para filtrar.");
            }
            else if (fechaLimiteCerca < fechaActual)
            {
                ModelState.AddModelError(nameof(model.FechaLimiteCerca), "La fecha límite de aviso no puede ser anterior a la fecha actual.");
            }
            else
            {
                var result = await _ingredienteNeveraService.FiltrarNeveraAsync(
                    p_nevera_oid, p_tipo, fechaLimiteCerca, fechaActual, p_estado);
                model.Resultados = result.Select(IngredienteNeveraAssembler.ToViewModel).ToList();
            }
        }

        return model;
    }

    // ------------------------------------------------------------------
    // CambiarCantidad (CU-05: Modificar cantidad + / - desde Mi Nevera)
    // ------------------------------------------------------------------

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarCantidad(int p_oid, int delta)
    {
        var entity = await _ingredienteNeveraService.ReadOIDAsync(p_oid);
        if (entity is null) return NotFound();
        if (!await CanAccessNeveraItemAsync(entity)) return Forbid();

        var nuevaCantidad = Math.Max(0, entity.Cantidad + delta);
        var nombre = entity.IngredienteCatalogo?.Nombre ?? $"#{entity.Id}";

        try
        {
            await _ingredienteNeveraService.ModifyAsync(p_oid, nuevaCantidad, entity.FechaCaducidad);
            TempData["Success"] = $"Cantidad de «{nombre}» actualizada a {nuevaCantidad}.";
        }
        catch (ModelException error)
        {
            TempData["Error"] = error.InnerException?.Message ?? error.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    /*PROTECTED REGION ID(IngredienteNeveraController_ComprobarCaducidad_POST_attributes) START*/
    /*PROTECTED REGION END*/
    public async Task<IActionResult> ComprobarCaducidad(int p_oid)
    {
    /*PROTECTED REGION ID(IngredienteNeveraController_ComprobarCaducidad_POST_body) ENABLED START*/

        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Identificador de ingrediente no válido.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var preEntity = await _ingredienteNeveraService.ReadOIDAsync(p_oid);
            if (preEntity is null) return NotFound();
            if (!await CanAccessNeveraItemAsync(preEntity)) return Forbid();

            await _ingredienteNeveraService.ComprobarCaducidadAsync(p_oid);
            var entity = await _ingredienteNeveraService.ReadOIDAsync(p_oid);
            if (entity is not null)
            {
                var nombre = entity.IngredienteCatalogo?.Nombre ?? $"#{entity.Id}";
                var hoy = DateTime.UtcNow.Date;
                var fecha = entity.FechaCaducidad.Date;
                if (fecha < hoy)
                {
                    TempData["Error"] = $"«{nombre}» está CADUCADO (venció el {fecha:yyyy-MM-dd}). Se ha descartado su stock disponible (cantidad = {entity.Cantidad}).";
                }
                else if (fecha <= hoy.AddDays(3))
                {
                    TempData["Warning"] = $"«{nombre}» está CERCA DE CADUCAR (vence el {fecha:yyyy-MM-dd}). Cantidad disponible: {entity.Cantidad}.";
                }
                else
                {
                    TempData["Success"] = $"«{nombre}» está en PERFECTO estado (vence el {fecha:yyyy-MM-dd}). Cantidad disponible: {entity.Cantidad}.";
                }
            }
            else
            {
                TempData["Success"] = "Caducidad comprobada correctamente.";
            }
        }
        catch (ModelException error)
        {
            TempData["Error"] = error.InnerException?.Message ?? error.Message;
        }

        return RedirectToAction(nameof(Index));
    /*PROTECTED REGION END*/
    }
}
