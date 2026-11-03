using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Application.Services;

public sealed partial class IngredienteCompraService : ServiceEntity<IngredienteCompra>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIngredienteCompraRepository _repository;

    public IngredienteCompraService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public IngredienteCompraService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private IngredienteCompraService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.IngredienteCompraRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.IngredienteCompraRepository;
    }

    public Task<IngredienteCompra> AddIngredienteCompraDefaultAsync(IngredienteCompra entity) =>
        AddDefaultAsync(entity);

    public Task DeleteIngredienteCompraDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<IngredienteCompra?> ReadOIDIngredienteCompraDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<IngredienteCompra>> ReadAllIngredienteCompraDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<IngredienteCompra> ModifierIngredienteCompraDefaultAsync(IngredienteCompra entity) =>
        ModifierDefaultAsync(entity);

    public async Task<IngredienteCompra> NewAsync(int p_cantidad, DateTime p_fechaCaducidad, bool p_comprado, int p_nevera_oid, int p_listaCompra_oid, int p_ingredienteCatalogo_oid)
    {
        Nevera neveraEntity = await _unitOfWork.NeveraRepository.ReadAsync(p_nevera_oid)
            ?? throw new ModelException("Nevera not found for IngredienteCompra.Nevera.");

        ListaCompra listaCompraEntity = await _unitOfWork.ListaCompraRepository.ReadAsync(p_listaCompra_oid)
            ?? throw new ModelException("ListaCompra not found for IngredienteCompra.ListaCompra.");

        Ingrediente ingredienteCatalogoEntity = await _unitOfWork.IngredienteRepository.ReadAsync(p_ingredienteCatalogo_oid)
            ?? throw new ModelException("Ingrediente not found for IngredienteCompra.IngredienteCatalogo.");

        IngredienteCompra entity = new IngredienteCompra
        {
            Cantidad = p_cantidad,
            Comprado = p_comprado,
            Nevera = neveraEntity,
            ListaCompra = listaCompraEntity,
            IngredienteCatalogo = ingredienteCatalogoEntity,
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        IngredienteCompra entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("IngredienteCompra not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, int p_cantidad, DateTime p_fechaCaducidad, bool p_comprado, int p_nevera_oid, int p_listaCompra_oid, int p_ingredienteCatalogo_oid)
    {
        IngredienteCompra entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("IngredienteCompra not found.");
        entity.Cantidad = p_cantidad;
        entity.Comprado = p_comprado;
        await _repository.SaveAsync(entity);
    }

    public async Task<IngredienteCompra?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<IngredienteCompra>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
