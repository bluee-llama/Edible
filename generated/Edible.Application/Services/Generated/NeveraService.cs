using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Application.Services;

public sealed partial class NeveraService : ServiceEntity<Nevera>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INeveraRepository _repository;

    public NeveraService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public NeveraService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private NeveraService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.NeveraRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.NeveraRepository;
    }

    public Task<Nevera> AddNeveraDefaultAsync(Nevera entity) =>
        AddDefaultAsync(entity);

    public Task DeleteNeveraDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Nevera?> ReadOIDNeveraDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Nevera>> ReadAllNeveraDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Nevera> ModifierNeveraDefaultAsync(Nevera entity) =>
        ModifierDefaultAsync(entity);

    public async Task<Nevera> NewAsync(int p_usuario_oid)
    {
        Usuario usuarioEntity = await _unitOfWork.UsuarioRepository.ReadAsync(p_usuario_oid)
            ?? throw new ModelException("Usuario not found for Nevera.Usuario.");

        Nevera entity = new Nevera
        {
            Usuario = usuarioEntity,
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        Nevera entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Nevera not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid)
    {
        Nevera entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Nevera not found.");
        await _repository.SaveAsync(entity);
    }

    public async Task<Nevera?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Nevera>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
