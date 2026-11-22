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

    public async Task ApplyRecetaRelationsAsync(Receta entity, int? p_chef_id, IReadOnlyList<Paso> p_pasos, IReadOnlyList<LineaReceta> p_lineas)
    {
        if (p_chef_id is null) throw new ModelException("Receta.Chef is required.");
        entity.Chef = p_chef_id is int selectedChef
            ? await _unitOfWork.ChefRepository.ReadAsync(selectedChef) ?? throw new ModelException("Chef not found for Receta.Chef.") : null!;
        if (p_pasos.Count < 0 || p_pasos.Count > 500) throw new ModelException("Invalid number of Pasos rows.");
        foreach (var removed in entity.Pasos.Where(child => !p_pasos.Contains(child)).ToList()) entity.Pasos.Remove(removed);
        foreach (var child in p_pasos)
        {
            child.Receta = entity;
            if (!entity.Pasos.Contains(child)) entity.Pasos.Add(child);
        }
        if (p_lineas.Count < 1 || p_lineas.Count > 500) throw new ModelException("Invalid number of Lineas rows.");
        foreach (var removed in entity.Lineas.Where(child => !p_lineas.Contains(child)).ToList()) entity.Lineas.Remove(removed);
        foreach (var child in p_lineas)
        {
            child.Receta = entity;
            if (!entity.Lineas.Contains(child)) entity.Lineas.Add(child);
        }
    }

    public async Task<List<Paso>> PrepareRecetaPasosAsync(Receta? parent, IReadOnlyList<(int Id, Paso Values)> rows)
    {
        if (rows.Count < 0 || rows.Count > 500) throw new ModelException("Invalid number of Pasos rows.");
        var seen = new HashSet<int>();
        var result = new List<Paso>();
        foreach (var row in rows)
        {
            var existing = !EqualityComparer<int>.Default.Equals(row.Id, default!);
            if (existing && (parent is null || !seen.Add(row.Id) || !parent.Pasos.Any(child => child.Id.Equals(row.Id))))
                throw new ModelException("The row does not belong to this record or is duplicated.");
            var child = existing ? parent!.Pasos.Single(item => item.Id.Equals(row.Id)) : new Paso();
            child.Nombre = row.Values.Nombre;
            child.Descripcion = row.Values.Descripcion;
            result.Add(child);
        }
        return result;
    }

    public async Task<List<LineaReceta>> PrepareRecetaLineasAsync(Receta? parent, IReadOnlyList<(int Id, LineaReceta Values, int? IngredienteCatalogoId)> rows)
    {
        if (rows.Count < 1 || rows.Count > 500) throw new ModelException("Invalid number of Lineas rows.");
        var seen = new HashSet<int>();
        var result = new List<LineaReceta>();
        foreach (var row in rows)
        {
            var existing = !EqualityComparer<int>.Default.Equals(row.Id, default!);
            if (existing && (parent is null || !seen.Add(row.Id) || !parent.Lineas.Any(child => child.Id.Equals(row.Id))))
                throw new ModelException("The row does not belong to this record or is duplicated.");
            var child = existing ? parent!.Lineas.Single(item => item.Id.Equals(row.Id)) : new LineaReceta();
            child.CantidadNecesaria = row.Values.CantidadNecesaria;
            if (row.IngredienteCatalogoId is null) throw new ModelException("LineaReceta.IngredienteCatalogo is required.");
            child.IngredienteCatalogo = row.IngredienteCatalogoId is int selectedIngredienteCatalogo
                ? await _unitOfWork.IngredienteRepository.ReadAsync(selectedIngredienteCatalogo) ?? throw new ModelException("Ingrediente not found for LineaReceta.IngredienteCatalogo.") : null!;
            result.Add(child);
        }
        return result;
    }

    public async Task ApplyLineaRecetaRelationsAsync(LineaReceta entity, int? p_receta_id, int? p_ingredienteCatalogo_id)
    {
        if (p_receta_id is null) throw new ModelException("LineaReceta.Receta is required.");
        entity.Receta = p_receta_id is int selectedReceta
            ? await _unitOfWork.RecetaRepository.ReadAsync(selectedReceta) ?? throw new ModelException("Receta not found for LineaReceta.Receta.") : null!;
        if (p_ingredienteCatalogo_id is null) throw new ModelException("LineaReceta.IngredienteCatalogo is required.");
        entity.IngredienteCatalogo = p_ingredienteCatalogo_id is int selectedIngredienteCatalogo
            ? await _unitOfWork.IngredienteRepository.ReadAsync(selectedIngredienteCatalogo) ?? throw new ModelException("Ingrediente not found for LineaReceta.IngredienteCatalogo.") : null!;
    }

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
