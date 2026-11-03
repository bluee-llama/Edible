using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Application.Services;

public sealed partial class IngredienteService : ServiceEntity<Ingrediente>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IIngredienteRepository _repository;

    public IngredienteService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public IngredienteService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private IngredienteService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.IngredienteRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.IngredienteRepository;
    }

    public Task<Ingrediente> AddIngredienteDefaultAsync(Ingrediente entity) =>
        AddDefaultAsync(entity);

    public Task DeleteIngredienteDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Ingrediente?> ReadOIDIngredienteDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Ingrediente>> ReadAllIngredienteDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Ingrediente> ModifierIngredienteDefaultAsync(Ingrediente entity) =>
        ModifierDefaultAsync(entity);

    public async Task<Ingrediente> NewAsync(string p_nombre, TipoIngrediente p_tipo, int p_calorias, float p_g_proteinas, float p_g_carbohidratos, float p_g_grasas, float p_g_racion, IReadOnlyList<Alergenos> p_alergenos, IReadOnlyList<DietaEspecial> p_dietaEspecial)
    {
        Ingrediente entity = new Ingrediente
        {
            Nombre = p_nombre,
            Tipo = p_tipo,
            Calorias = p_calorias,
            G_proteinas = p_g_proteinas,
            G_carbohidratos = p_g_carbohidratos,
            G_grasas = p_g_grasas,
            G_racion = p_g_racion,
            Alergenos = new List<Alergenos>(p_alergenos),
            DietaEspecial = new List<DietaEspecial>(p_dietaEspecial),
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        Ingrediente entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Ingrediente not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, string p_nombre, TipoIngrediente p_tipo, int p_calorias, float p_g_proteinas, float p_g_carbohidratos, float p_g_grasas, float p_g_racion, IReadOnlyList<Alergenos> p_alergenos, IReadOnlyList<DietaEspecial> p_dietaEspecial)
    {
        Ingrediente entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Ingrediente not found.");
        entity.Nombre = p_nombre;
        entity.Tipo = p_tipo;
        entity.Calorias = p_calorias;
        entity.G_proteinas = p_g_proteinas;
        entity.G_carbohidratos = p_g_carbohidratos;
        entity.G_grasas = p_g_grasas;
        entity.G_racion = p_g_racion;
        entity.Alergenos = new List<Alergenos>(p_alergenos);
        entity.DietaEspecial = new List<DietaEspecial>(p_dietaEspecial);
        await _repository.SaveAsync(entity);
    }

    public async Task<Ingrediente?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Ingrediente>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
