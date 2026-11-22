using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Application.Services;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

        /*PROTECTED REGION ID(usingEdible.Application.UseCases_Receta_comprobarIngredientesDisponibles) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace Edible.Application.UseCases.Receta;

public sealed partial class RecetaUseCase
{
    public async Task<global::Edible.Domain.Entities.Receta> ComprobarIngredientesDisponiblesAsync(string p_nombre, string p_descripcion, global::Edible.Domain.Enums.DietaEspecial p_dietaEspecial, float p_valoracionMedia, int p_chef_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.UseCases_Receta_comprobarIngredientesDisponibles) ENABLED START*/
        // Resolve external infrastructure services with _utils!.GetRequiredService<TService>() when this use case receives IUnitOfWorkUtils.
        RecetaService recetaService = null!;
        global::Edible.Domain.Entities.Receta result = default(global::Edible.Domain.Entities.Receta)!;

        try
        {
            await _unitOfWork.BeginTransactionAsync("RecetaUseCase.ComprobarIngredientesDisponiblesAsync");
            recetaService = new RecetaService(_unitOfWork);

            // Write here your custom transaction code...
            // Use recetaService, _unitOfWork.RecetaRepository or _utils!.GetRequiredService<TService>() as the generated CP equivalent.

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
