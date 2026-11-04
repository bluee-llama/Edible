using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Application.Services;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

        /*PROTECTED REGION ID(usingEdible.Application.UseCases_Comentario_recalcularValoracionMedia) ENABLED START*/
        using System.Linq;
        /*PROTECTED REGION END*/

namespace Edible.Application.UseCases.Comentario;

public sealed partial class ComentarioUseCase
{
    public async Task<global::Edible.Domain.Entities.Comentario> RecalcularValoracionMediaAsync(int p_user_id, string p_titulo, int p_valoracion, int p_comentador_oid, int p_roleB_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.UseCases_Comentario_recalcularValoracionMedia) ENABLED START*/
        ComentarioService comentarioService = null!;
        global::Edible.Domain.Entities.Comentario result = default(global::Edible.Domain.Entities.Comentario)!;

        try
        {
            await _unitOfWork.BeginTransactionAsync("ComentarioUseCase.RecalcularValoracionMediaAsync");
            comentarioService = new ComentarioService(_unitOfWork);

            // Precondiciones
            if (string.IsNullOrWhiteSpace(p_titulo))
                throw new ModelException("El título del comentario no puede estar vacío.");
            if (p_valoracion < 1 || p_valoracion > 5)
                throw new ModelException("La valoración debe estar entre 1 y 5.");

            // Crea el comentario (NewAsync ya comprueba que existan el usuario y la receta)
            result = await comentarioService.NewAsync(p_user_id, p_titulo, p_valoracion, p_comentador_oid, p_roleB_oid);

            var receta = result.Receta;

            // Comentarios de ESTA receta (incluido el recién creado)
            var todos = await _unitOfWork.ComentarioRepository.ReadAllAsync();
            var comentariosReceta = todos.Where(c => c.Receta != null && c.Receta.Id == receta.Id).ToList();
            if (!comentariosReceta.Contains(result))
                comentariosReceta.Add(result);

            double media = comentariosReceta.Average(c => c.Valoracion);
            receta.ValoracionMedia = (float)media;

            // Postcondición
            if (receta.ValoracionMedia < 1f || receta.ValoracionMedia > 5f)
                throw new ModelException("Postcondición incumplida: la valoración media debe estar entre 1 y 5.");

            await _unitOfWork.RecetaRepository.SaveAsync(receta);

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