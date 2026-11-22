using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Application.Services;

public sealed partial class ChefService : ServiceEntity<Chef>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IChefRepository _repository;
    private readonly IHariaPasswordHasher _passwordHasher;

    public ChefService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public ChefService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private ChefService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.ChefRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.ChefRepository;
        _passwordHasher = unitOfWork.PasswordHasher;
    }

    public Task<Chef> AddChefDefaultAsync(Chef entity)
    {
        HashPasswordProperties(entity);
        return AddDefaultAsync(entity);
    }

    public Task DeleteChefDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Chef?> ReadOIDChefDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Chef>> ReadAllChefDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Chef> ModifierChefDefaultAsync(Chef entity)
    {
        HashPasswordProperties(entity);
        return ModifierDefaultAsync(entity);
    }

    private void HashPasswordProperties(Chef entity)
    {
        if (entity.Password is not null)
        {
            entity.Password = _passwordHasher.HashPasswordIfNeeded(entity.Password);
        }
    }

    public async Task ApplyChefRelationsAsync(Chef entity, int? p_nevera_id, int? p_listaCompra_id)
    {
        if (p_nevera_id is null) throw new ModelException("Chef.Nevera is required.");
        entity.Nevera = p_nevera_id is int selectedNevera
            ? await _unitOfWork.NeveraRepository.ReadAsync(selectedNevera) ?? throw new ModelException("Nevera not found for Chef.Nevera.") : null!;
        if (p_listaCompra_id is null) throw new ModelException("Chef.ListaCompra is required.");
        entity.ListaCompra = p_listaCompra_id is int selectedListaCompra
            ? await _unitOfWork.ListaCompraRepository.ReadAsync(selectedListaCompra) ?? throw new ModelException("ListaCompra not found for Chef.ListaCompra.") : null!;
    }

    public async Task<Chef> NewAsync(string p_name, string p_password, bool p_publico, DietaEspecial p_dieta, Nevera p_nevera, ListaCompra p_listaCompra)
    {
        Chef entity = new Chef
        {
            Name = p_name,
            Password = _passwordHasher.HashPasswordIfNeeded(p_password),
            Publico = p_publico,
            Dieta = p_dieta,
            Nevera = p_nevera,
            ListaCompra = p_listaCompra,
        };

        entity.Nevera.Usuario = entity;

        entity.ListaCompra.Usuario = entity;

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        Chef entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Chef not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, string p_name, string p_password, bool p_publico, DietaEspecial p_dieta)
    {
        Chef entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Chef not found.");
        entity.Name = p_name;
        entity.Password = _passwordHasher.HashPasswordIfNeeded(p_password);
        entity.Publico = p_publico;
        entity.Dieta = p_dieta;
        await _repository.SaveAsync(entity);
    }

    public async Task<Chef?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Chef>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
