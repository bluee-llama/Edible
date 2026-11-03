using Edible.Application.Abstractions;

namespace Edible.Application.UseCases.Receta;

public sealed partial class RecetaUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUnitOfWorkUtils? _utils;

    public RecetaUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public RecetaUseCase(IUnitOfWork unitOfWork, IUnitOfWorkUtils utils)
        : this(unitOfWork)
    {
        _utils = utils;
    }
}
