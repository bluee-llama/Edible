using Edible.Application.Abstractions;

namespace Edible.Application.UseCases.Comentario;

public sealed partial class ComentarioUseCase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUnitOfWorkUtils? _utils;

    public ComentarioUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public ComentarioUseCase(IUnitOfWork unitOfWork, IUnitOfWorkUtils utils)
        : this(unitOfWork)
    {
        _utils = utils;
    }
}
