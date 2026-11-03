using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Application.Services;

public sealed partial class RecetaService : ServiceEntity<Receta>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IRecetaRepository _repository;

    public RecetaService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public RecetaService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private RecetaService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.RecetaRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.RecetaRepository;
    }

    public Task<Receta> AddRecetaDefaultAsync(Receta entity) =>
        AddDefaultAsync(entity);

    public Task DeleteRecetaDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Receta?> ReadOIDRecetaDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Receta>> ReadAllRecetaDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Receta> ModifierRecetaDefaultAsync(Receta entity) =>
        ModifierDefaultAsync(entity);

    public async Task<Receta> NewAsync(string p_nombre, string p_descripcion, DietaEspecial p_dietaEspecial, float p_valoracionMedia, int p_tiempoRealizacion, int p_dificultad, int p_chef_oid, IReadOnlyList<LineaReceta> p_lineas)
    {
        Chef chefEntity = await _unitOfWork.ChefRepository.ReadAsync(p_chef_oid)
            ?? throw new ModelException("Chef not found for Receta.Chef.");

        Receta entity = new Receta
        {
            Nombre = p_nombre,
            Descripcion = p_descripcion,
            DietaEspecial = p_dietaEspecial,
            ValoracionMedia = p_valoracionMedia,
            TiempoRealizacion = p_tiempoRealizacion,
            Dificultad = p_dificultad,
            Chef = chefEntity,
            Lineas = p_lineas.ToList(),
        };

        foreach (var recetaLineaReceta in entity.Lineas)
        {
            recetaLineaReceta.Receta = entity;
        }

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        Receta entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Receta not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, string p_nombre, string p_descripcion, DietaEspecial p_dietaEspecial, float p_valoracionMedia, int p_tiempoRealizacion, int p_dificultad, int p_chef_oid)
    {
        Receta entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Receta not found.");
        entity.Nombre = p_nombre;
        entity.Descripcion = p_descripcion;
        entity.DietaEspecial = p_dietaEspecial;
        entity.ValoracionMedia = p_valoracionMedia;
        entity.TiempoRealizacion = p_tiempoRealizacion;
        entity.Dificultad = p_dificultad;
        await _repository.SaveAsync(entity);
    }

    public async Task<Receta?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Receta>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

    public async Task<LineaReceta> NewLineaRecetaAsync(int p_cantidadNecesaria, int p_receta_oid, int p_ingrediente_oid)
    {
        Receta recetaEntity = await _unitOfWork.RecetaRepository.ReadAsync(p_receta_oid)
            ?? throw new ModelException("Receta not found for LineaReceta.Receta.");

        Ingrediente ingredienteCatalogoEntity = await _unitOfWork.IngredienteRepository.ReadAsync(p_ingrediente_oid)
            ?? throw new ModelException("Ingrediente not found for LineaReceta.IngredienteCatalogo.");

        LineaReceta entity = new LineaReceta
        {
            CantidadNecesaria = p_cantidadNecesaria,
            Receta = recetaEntity,
            IngredienteCatalogo = ingredienteCatalogoEntity,
        };

        await _unitOfWork.LineaRecetaRepository.SaveAsync(entity);
        return entity;
    }

    public async Task ModifyLineaRecetaAsync(int p_oid, int p_cantidadNecesaria, int p_receta_oid, int p_ingrediente_oid)
    {
        LineaReceta entity = await _unitOfWork.LineaRecetaRepository.ReadAsync(p_oid)
            ?? throw new ModelException("LineaReceta not found.");
        entity.CantidadNecesaria = p_cantidadNecesaria;
        await _unitOfWork.LineaRecetaRepository.SaveAsync(entity);
    }

    public async Task DestroyLineaRecetaAsync(int p_oid)
    {
        LineaReceta entity = await _unitOfWork.LineaRecetaRepository.ReadAsync(p_oid)
            ?? throw new ModelException("LineaReceta not found.");
        await _unitOfWork.LineaRecetaRepository.DeleteAsync(entity);
    }

    public async Task<LineaReceta?> ReadOIDLineaRecetaAsync(int p_oid)
    {
        return await _unitOfWork.LineaRecetaRepository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<LineaReceta>> ReadAllLineaRecetaAsync()
    {
        return await _unitOfWork.LineaRecetaRepository.ReadAllAsync();
    }

    public async Task<IReadOnlyList<Receta>> BusquedaAvanzadaAsync(DietaEspecial p_dieta, float p_notaMinima, int p_tiempoMin, int p_tiempoMax, int p_dificultad, Alergenos p_alergenoExcluido, int p_ingrediente_oid)
    {
        return await _repository.BusquedaAvanzadaAsync(p_dieta, p_notaMinima, p_tiempoMin, p_tiempoMax, p_dificultad, p_alergenoExcluido, p_ingrediente_oid);
    }

    public async Task<IReadOnlyList<Receta>> BuscarNombreAsync(string p_nombre)
    {
        return await _repository.BuscarNombreAsync(p_nombre);
    }

    public async Task<IReadOnlyList<Receta>> RecetasPorNeveraAsync(int p_nevera_oid)
    {
        return await _repository.RecetasPorNeveraAsync(p_nevera_oid);
    }

}
