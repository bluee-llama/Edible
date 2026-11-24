using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using System;

/*PROTECTED REGION ID(usingEdible.Application.Services_IngredienteNevera_comprobarCaducidad) ENABLED START*/
using System;
using Edible.Domain.Enums;
/*PROTECTED REGION END*/

namespace Edible.Application.Services;

public sealed partial class IngredienteNeveraService
{
    public async Task ComprobarCaducidadAsync(int p_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.Services_IngredienteNevera_comprobarCaducidad) ENABLED START*/
        var entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("El ingrediente en nevera no existe.");

        var fechaActual = DateTime.UtcNow.Date;
        var fechaLimiteCerca = fechaActual.AddDays(3);
        var fechaCaducidad = entity.FechaCaducidad.Date;

        EstadoCaducidad estado;
        if (fechaCaducidad < fechaActual)
        {
            estado = EstadoCaducidad.caducado;
            // Si el ingrediente ha caducado, se descarta su stock disponible en nevera.
            entity.Cantidad = 0;
        }
        else if (fechaCaducidad <= fechaLimiteCerca)
        {
            estado = EstadoCaducidad.cerca;
        }
        else
        {
            estado = EstadoCaducidad.perfecto;
        }

        _ = estado;
        await _repository.SaveAsync(entity);
        /*PROTECTED REGION END*/
    }
}