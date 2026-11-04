using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
<<<<<<< HEAD
using System;

/*PROTECTED REGION ID(usingEdible.Application.Services_IngredienteNevera_comprobarCaducidad) ENABLED START*/
// references to other libraries
/*PROTECTED REGION END*/
=======
using Edible.Domain.Enums;

        // references to other libraries
>>>>>>> bbdcbb95ef26f24cd06e4610fa13c9b01fb6d5d4

namespace Edible.Application.Services;

public sealed partial class IngredienteNeveraService
{
<<<<<<< HEAD
    public async Task ComprobarCaducidadAsync(int p_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.Services_IngredienteNevera_comprobarCaducidad) ENABLED START*/
        var entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("El ingrediente en nevera no existe.");

        // Se persiste la lectura transaccional respetando los atributos del modelo TDM oficial.
        await _repository.SaveAsync(entity);
        /*PROTECTED REGION END*/
    }
}
=======
    /*PROTECTED REGION ID(usingEdible.Application.Services_IngredienteNevera_comprobarCaducidad) ENABLED START*/

    public async Task ComprobarCaducidadAsync(int p_oid)
    {
        var item = await _repository.ReadAsync(p_oid) 
            ?? throw new ModelException("El ingrediente de la nevera no existe.");

        var hoy = DateTime.UtcNow.Date;
        var diasRestantes = (item.FechaCaducidad.Date - hoy).TotalDays;

        if (diasRestantes < 0)
        {
            item.EstadoCaducidad = EstadoCaducidad.Caducado;
        }
        else if (diasRestantes <= 3)
        {
            item.EstadoCaducidad = EstadoCaducidad.CercaCaducar;
        }
        else
        {
            item.EstadoCaducidad = EstadoCaducidad.Optimo;
        }

        await _repository.SaveAsync(item);
    }
    /*PROTECTED REGION END*/

}
>>>>>>> bbdcbb95ef26f24cd06e4610fa13c9b01fb6d5d4
