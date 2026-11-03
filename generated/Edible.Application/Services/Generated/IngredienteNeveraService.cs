using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Application.Services;

public sealed partial class IngredienteNeveraService : ServiceEntity<IngredienteNevera>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIngredienteNeveraRepository _repository;

    public IngredienteNeveraService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public IngredienteNeveraService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private IngredienteNeveraService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.IngredienteNeveraRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.IngredienteNeveraRepository;
    }

    public Task<IngredienteNevera> AddIngredienteNeveraDefaultAsync(IngredienteNevera entity) =>
        AddDefaultAsync(entity);

    public Task DeleteIngredienteNeveraDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<IngredienteNevera?> ReadOIDIngredienteNeveraDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<IngredienteNevera>> ReadAllIngredienteNeveraDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<IngredienteNevera> ModifierIngredienteNeveraDefaultAsync(IngredienteNevera entity) =>
        ModifierDefaultAsync(entity);

    public async Task<IngredienteNevera> NewAsync(int p_cantidad, DateTime p_fechaCaducidad, int p_ingredienteCatalogo_oid, int p_nevera_oid, int p_receta_oid)
    {
        Ingrediente ingredienteCatalogoEntity = await _unitOfWork.IngredienteRepository.ReadAsync(p_ingredienteCatalogo_oid)
            ?? throw new ModelException("Ingrediente not found for IngredienteNevera.IngredienteCatalogo.");

        Nevera neveraEntity = await _unitOfWork.NeveraRepository.ReadAsync(p_nevera_oid)
            ?? throw new ModelException("Nevera not found for IngredienteNevera.Nevera.");

        Receta recetaEntity = await _unitOfWork.RecetaRepository.ReadAsync(p_receta_oid)
            ?? throw new ModelException("Receta not found for IngredienteNevera.Receta.");

        IngredienteNevera entity = new IngredienteNevera
        {
            Cantidad = p_cantidad,
            FechaCaducidad = p_fechaCaducidad,
            IngredienteCatalogo = ingredienteCatalogoEntity,
            Nevera = neveraEntity,
            Receta = recetaEntity,
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        IngredienteNevera entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("IngredienteNevera not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, int p_cantidad, DateTime p_fechaCaducidad)
    {
        IngredienteNevera entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("IngredienteNevera not found.");
        entity.Cantidad = p_cantidad;
        entity.FechaCaducidad = p_fechaCaducidad;
        await _repository.SaveAsync(entity);
    }

    public async Task<IngredienteNevera?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<IngredienteNevera>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

    public async Task<IReadOnlyList<IngredienteNevera>> FiltrarNeveraAsync(int p_nevera_oid, TipoIngrediente p_tipo, DateTime p_fechaLimiteCerca, DateTime p_fechaActual, EstadoCaducidad p_estado)
    {
        return await _repository.FiltrarNeveraAsync(p_nevera_oid, p_tipo, p_fechaLimiteCerca, p_fechaActual, p_estado);
    }

}
