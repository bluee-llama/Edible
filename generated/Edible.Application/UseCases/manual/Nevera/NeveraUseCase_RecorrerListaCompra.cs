using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Application.Services;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

        /*PROTECTED REGION ID(usingEdible.Application.UseCases_Nevera_recorrerListaCompra) ENABLED START*/
        // references to other libraries
        /*PROTECTED REGION END*/

namespace Edible.Application.UseCases.Nevera;

public sealed partial class NeveraUseCase
{
    public async Task<global::Edible.Domain.Entities.Nevera> RecorrerListaCompraAsync(int p_usuario_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.UseCases_Nevera_recorrerListaCompra) ENABLED START*/
        // Resolve external infrastructure services with _utils!.GetRequiredService<TService>() when this use case receives IUnitOfWorkUtils.
        NeveraService neveraService = null!;
        global::Edible.Domain.Entities.Nevera result = default(global::Edible.Domain.Entities.Nevera)!;

        try
        {
            await _unitOfWork.BeginTransactionAsync("NeveraUseCase.RecorrerListaCompraAsync");
            neveraService = new NeveraService(_unitOfWork);

            // Write here your custom transaction code...
            // Use neveraService, _unitOfWork.NeveraRepository or _utils!.GetRequiredService<TService>() as the generated CP equivalent.

            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new ModelException("Application error while executing use case Nevera.recorrerListaCompra.", ex);
        }

        return result;
        /*PROTECTED REGION END*/
    }
}
