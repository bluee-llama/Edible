using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Application.Services;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using System.Linq;

/*PROTECTED REGION ID(usingEdible.Application.UseCases_Receta_comprobarIngredientesDisponibles) ENABLED START*/
// references to other libraries
/*PROTECTED REGION END*/

namespace Edible.Application.UseCases.Receta;

public sealed partial class RecetaUseCase
{
    public async Task<global::Edible.Domain.Entities.Receta> ComprobarIngredientesDisponiblesAsync(string p_nombre, string p_descripcion, global::Edible.Domain.Enums.DietaEspecial p_dietaEspecial, float p_valoracionMedia, int p_chef_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.UseCases_Receta_comprobarIngredientesDisponibles) ENABLED START*/
        RecetaService recetaService = null!;
        global::Edible.Domain.Entities.Receta result = default(global::Edible.Domain.Entities.Receta)!;

       try
        {
            await _unitOfWork.BeginTransactionAsync("RecetaUseCase.ComprobarIngredientesDisponiblesAsync");
            recetaService = new RecetaService(_unitOfWork);

            int idRecetaObjetivo = 1; 
            int idNeveraObjetivo = 1; 

            result = await _unitOfWork.RecetaRepository.ReadAsync(idRecetaObjetivo)
                ?? throw new ModelException("La receta indicada no existe.");

            // SOLUCIÓN: Usamos ReadAllAsync y filtramos en memoria
            var todasLineas = await _unitOfWork.LineaRecetaRepository.ReadAllAsync();
            var lineasReceta = todasLineas.Where(l => l.Receta != null && l.Receta.Id == idRecetaObjetivo).ToList();

            var todosIngredientes = await _unitOfWork.IngredienteNeveraRepository.ReadAllAsync();
            var ingredientesNevera = todosIngredientes.Where(i => i.Nevera != null && i.Nevera.Id == idNeveraObjetivo).ToList();

           foreach (var linea in lineasReceta)
            {
                // Usamos IngredienteCatalogo y CantidadNecesaria
                var ingredienteDisponible = ingredientesNevera.FirstOrDefault(i => 
                    i.IngredienteCatalogo != null && 
                    linea.IngredienteCatalogo != null && 
                    i.IngredienteCatalogo.Id == linea.IngredienteCatalogo.Id);
                
                if (ingredienteDisponible == null || ingredienteDisponible.Cantidad < linea.CantidadNecesaria)
                {
                    throw new ModelException($"Falta stock del ingrediente ID: {linea.IngredienteCatalogo?.Id}");
                }
            }
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new ModelException("Application error while executing use case Receta.comprobarIngredientesDisponibles.", ex);
        }

        return result;
        /*PROTECTED REGION END*/
    }
}