using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Application.Services;

public sealed partial class ListaCompraService : ServiceEntity<ListaCompra>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IListaCompraRepository _repository;

    public ListaCompraService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public ListaCompraService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private ListaCompraService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.ListaCompraRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.ListaCompraRepository;
    }

    public Task<ListaCompra> AddListaCompraDefaultAsync(ListaCompra entity) =>
        AddDefaultAsync(entity);

    public Task DeleteListaCompraDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<ListaCompra?> ReadOIDListaCompraDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<ListaCompra>> ReadAllListaCompraDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<ListaCompra> ModifierListaCompraDefaultAsync(ListaCompra entity) =>
        ModifierDefaultAsync(entity);

    public async Task<ListaCompra> NewAsync(int p_usuario_oid)
    {
        Usuario usuarioEntity = await _unitOfWork.UsuarioRepository.ReadAsync(p_usuario_oid)
            ?? throw new ModelException("Usuario not found for ListaCompra.Usuario.");

        ListaCompra entity = new ListaCompra
        {
            Usuario = usuarioEntity,
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        ListaCompra entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("ListaCompra not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid)
    {
        ListaCompra entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("ListaCompra not found.");
        await _repository.SaveAsync(entity);
    }

    public async Task<ListaCompra?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<ListaCompra>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
