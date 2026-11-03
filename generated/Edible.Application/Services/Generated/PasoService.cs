using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Application.Services;

public sealed partial class PasoService : ServiceEntity<Paso>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasoRepository _repository;

    public PasoService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public PasoService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private PasoService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.PasoRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.PasoRepository;
    }

    public Task<Paso> AddPasoDefaultAsync(Paso entity) =>
        AddDefaultAsync(entity);

    public Task DeletePasoDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Paso?> ReadOIDPasoDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Paso>> ReadAllPasoDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Paso> ModifierPasoDefaultAsync(Paso entity) =>
        ModifierDefaultAsync(entity);

    public async Task<Paso> NewAsync(string p_nombre, string p_descripcion, int p_receta_oid)
    {
        Receta recetaEntity = await _unitOfWork.RecetaRepository.ReadAsync(p_receta_oid)
            ?? throw new ModelException("Receta not found for Paso.Receta.");

        Paso entity = new Paso
        {
            Nombre = p_nombre,
            Descripcion = p_descripcion,
            Receta = recetaEntity,
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        Paso entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Paso not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, string p_nombre, string p_descripcion)
    {
        Paso entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Paso not found.");
        entity.Nombre = p_nombre;
        entity.Descripcion = p_descripcion;
        await _repository.SaveAsync(entity);
    }

    public async Task<Paso?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Paso>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
