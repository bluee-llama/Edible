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

    public async Task ApplyNeveraRelationsAsync(Nevera entity, int? p_usuario_id, IReadOnlyList<IngredienteCompra> p_ingredienteEnLaLista, IReadOnlyList<IngredienteNevera> p_ingredienteEnLaNevera)
    {
        if (p_usuario_id is null) throw new ModelException("Nevera.Usuario is required.");
        entity.Usuario = p_usuario_id is int selectedUsuario
            ? await _unitOfWork.UsuarioRepository.ReadAsync(selectedUsuario) ?? throw new ModelException("Usuario not found for Nevera.Usuario.") : null!;
        if (p_ingredienteEnLaLista.Count < 0 || p_ingredienteEnLaLista.Count > 500) throw new ModelException("Invalid number of IngredienteEnLaLista rows.");
        foreach (var removed in entity.IngredienteEnLaLista.Where(child => !p_ingredienteEnLaLista.Contains(child)).ToList()) entity.IngredienteEnLaLista.Remove(removed);
        foreach (var child in p_ingredienteEnLaLista)
        {
            child.Nevera = entity;
            if (!entity.IngredienteEnLaLista.Contains(child)) entity.IngredienteEnLaLista.Add(child);
        }
        if (p_ingredienteEnLaNevera.Count < 0 || p_ingredienteEnLaNevera.Count > 500) throw new ModelException("Invalid number of IngredienteEnLaNevera rows.");
        foreach (var removed in entity.IngredienteEnLaNevera.Where(child => !p_ingredienteEnLaNevera.Contains(child)).ToList()) entity.IngredienteEnLaNevera.Remove(removed);
        foreach (var child in p_ingredienteEnLaNevera)
        {
            child.Nevera = entity;
            if (!entity.IngredienteEnLaNevera.Contains(child)) entity.IngredienteEnLaNevera.Add(child);
        }
    }

    public async Task<List<IngredienteCompra>> PrepareNeveraIngredienteEnLaListaAsync(Nevera? parent, IReadOnlyList<(int Id, IngredienteCompra Values, int? IngredienteCatalogoId, int? ListaCompraId)> rows)
    {
        if (rows.Count < 0 || rows.Count > 500) throw new ModelException("Invalid number of IngredienteEnLaLista rows.");
        var seen = new HashSet<int>();
        var result = new List<IngredienteCompra>();
        foreach (var row in rows)
        {
            var existing = !EqualityComparer<int>.Default.Equals(row.Id, default!);
            if (existing && (parent is null || !seen.Add(row.Id) || !parent.IngredienteEnLaLista.Any(child => child.Id.Equals(row.Id))))
                throw new ModelException("The row does not belong to this record or is duplicated.");
            var child = existing ? parent!.IngredienteEnLaLista.Single(item => item.Id.Equals(row.Id)) : new IngredienteCompra();
            child.Cantidad = row.Values.Cantidad;
            child.Comprado = row.Values.Comprado;
            if (row.IngredienteCatalogoId is null) throw new ModelException("IngredienteCompra.IngredienteCatalogo is required.");
            child.IngredienteCatalogo = row.IngredienteCatalogoId is int selectedIngredienteCatalogo
                ? await _unitOfWork.IngredienteRepository.ReadAsync(selectedIngredienteCatalogo) ?? throw new ModelException("Ingrediente not found for IngredienteCompra.IngredienteCatalogo.") : null!;
            if (row.ListaCompraId is null) throw new ModelException("IngredienteCompra.ListaCompra is required.");
            child.ListaCompra = row.ListaCompraId is int selectedListaCompra
                ? await _unitOfWork.ListaCompraRepository.ReadAsync(selectedListaCompra) ?? throw new ModelException("ListaCompra not found for IngredienteCompra.ListaCompra.") : null!;
            result.Add(child);
        }
        return result;
    }

    public async Task<List<IngredienteNevera>> PrepareNeveraIngredienteEnLaNeveraAsync(Nevera? parent, IReadOnlyList<(int Id, IngredienteNevera Values, int? IngredienteCatalogoId, int? RecetaId)> rows)
    {
        if (rows.Count < 0 || rows.Count > 500) throw new ModelException("Invalid number of IngredienteEnLaNevera rows.");
        var seen = new HashSet<int>();
        var result = new List<IngredienteNevera>();
        foreach (var row in rows)
        {
            var existing = !EqualityComparer<int>.Default.Equals(row.Id, default!);
            if (existing && (parent is null || !seen.Add(row.Id) || !parent.IngredienteEnLaNevera.Any(child => child.Id.Equals(row.Id))))
                throw new ModelException("The row does not belong to this record or is duplicated.");
            var child = existing ? parent!.IngredienteEnLaNevera.Single(item => item.Id.Equals(row.Id)) : new IngredienteNevera();
            child.Cantidad = row.Values.Cantidad;
            child.FechaCaducidad = row.Values.FechaCaducidad;
            if (row.IngredienteCatalogoId is null) throw new ModelException("IngredienteNevera.IngredienteCatalogo is required.");
            child.IngredienteCatalogo = row.IngredienteCatalogoId is int selectedIngredienteCatalogo
                ? await _unitOfWork.IngredienteRepository.ReadAsync(selectedIngredienteCatalogo) ?? throw new ModelException("Ingrediente not found for IngredienteNevera.IngredienteCatalogo.") : null!;
            if (row.RecetaId is null) throw new ModelException("IngredienteNevera.Receta is required.");
            child.Receta = row.RecetaId is int selectedReceta
                ? await _unitOfWork.RecetaRepository.ReadAsync(selectedReceta) ?? throw new ModelException("Receta not found for IngredienteNevera.Receta.") : null!;
            result.Add(child);
        }
        return result;
    }

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
