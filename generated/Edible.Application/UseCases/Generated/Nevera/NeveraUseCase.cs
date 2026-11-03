using Edible.Application.Abstractions;

namespace Edible.Application.UseCases.Nevera;

public sealed partial class NeveraUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUnitOfWorkUtils? _utils;

    public NeveraUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public NeveraUseCase(IUnitOfWork unitOfWork, IUnitOfWorkUtils utils)
        : this(unitOfWork)
    {
        _utils = utils;
    }
}
