using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Application.Services;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

        /*PROTECTED REGION ID(usingEdible.Application.UseCases_Comentario_recalcularValoracionMedia) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace Edible.Application.UseCases.Comentario;

public sealed partial class ComentarioUseCase
{
    public async Task<global::Edible.Domain.Entities.Comentario> RecalcularValoracionMediaAsync(int p_user_id, string p_titulo, int p_valoracion, int p_comentador_oid, int p_roleB_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.UseCases_Comentario_recalcularValoracionMedia) ENABLED START*/
        // Resolve external infrastructure services with _utils!.GetRequiredService<TService>() when this use case receives IUnitOfWorkUtils.
        ComentarioService comentarioService = null!;
        global::Edible.Domain.Entities.Comentario result = default(global::Edible.Domain.Entities.Comentario)!;

        try
        {
            await _unitOfWork.BeginTransactionAsync("ComentarioUseCase.RecalcularValoracionMediaAsync");
            comentarioService = new ComentarioService(_unitOfWork);

            // Write here your custom transaction code...
            // Use comentarioService, _unitOfWork.ComentarioRepository or _utils!.GetRequiredService<TService>() as the generated CP equivalent.

            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new ModelException("Application error while executing use case Comentario.recalcularValoracionMedia.", ex);
        }

        return result;
        /*PROTECTED REGION END*/
    }
}
