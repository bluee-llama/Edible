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

            // NOTA: Los parámetros autogenerados (p_nombre, p_dietaEspecial...) indican que en el TDM 
            // definiste esta operación con argumentos de creación en lugar de identificadores (OID). 
            // Si el objetivo era solo comprobar el stock, deberías cambiar la firma en el TDM para 
            // recibir (int p_receta_oid, int p_nevera_oid) y devolver Boolean. 
            // Si mantienes esta firma, sustituye los valores estáticos por tu lógica de cruce de entidades.

            int idRecetaObjetivo = 1; // Ajustar según lógica de negocio real
            int idNeveraObjetivo = 1; // Ajustar consultando la nevera asociada a p_chef_oid

            result = await _unitOfWork.RecetaRepository.ReadAsync(idRecetaObjetivo)
                ?? throw new ModelException("La receta indicada no existe.");

            // Requiere que implementes los filtros LinQ en sus respectivos repositorios
            var lineasReceta = await _unitOfWork.LineaRecetaRepository.PorRecetaLinQAsync(idRecetaObjetivo);
            var ingredientesNevera = await _unitOfWork.IngredienteNeveraRepository.PorNeveraLinQAsync(idNeveraObjetivo);

            foreach (var linea in lineasReceta)
            {
                var ingredienteDisponible = ingredientesNevera.FirstOrDefault(i => i.Ingrediente.Id == linea.Ingrediente.Id);
                
                if (ingredienteDisponible == null || ingredienteDisponible.Cantidad < linea.Cantidad)
                {
                    throw new ModelException($"Falta stock del ingrediente ID: {linea.Ingrediente.Id}");
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