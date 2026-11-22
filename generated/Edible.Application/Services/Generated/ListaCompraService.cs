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

    public async Task ApplyListaCompraRelationsAsync(ListaCompra entity, int? p_usuario_id, IReadOnlyList<IngredienteCompra> p_ingredienteEnLaLista)
    {
        if (p_usuario_id is null) throw new ModelException("ListaCompra.Usuario is required.");
        entity.Usuario = p_usuario_id is int selectedUsuario
            ? await _unitOfWork.UsuarioRepository.ReadAsync(selectedUsuario) ?? throw new ModelException("Usuario not found for ListaCompra.Usuario.") : null!;
        if (p_ingredienteEnLaLista.Count < 0 || p_ingredienteEnLaLista.Count > 500) throw new ModelException("Invalid number of IngredienteEnLaLista rows.");
        foreach (var removed in entity.IngredienteEnLaLista.Where(child => !p_ingredienteEnLaLista.Contains(child)).ToList()) entity.IngredienteEnLaLista.Remove(removed);
        foreach (var child in p_ingredienteEnLaLista)
        {
            child.ListaCompra = entity;
            if (!entity.IngredienteEnLaLista.Contains(child)) entity.IngredienteEnLaLista.Add(child);
        }
    }

    public async Task<List<IngredienteCompra>> PrepareListaCompraIngredienteEnLaListaAsync(ListaCompra? parent, IReadOnlyList<(int Id, IngredienteCompra Values, int? IngredienteCatalogoId, int? NeveraId)> rows)
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
            if (row.NeveraId is null) throw new ModelException("IngredienteCompra.Nevera is required.");
            child.Nevera = row.NeveraId is int selectedNevera
                ? await _unitOfWork.NeveraRepository.ReadAsync(selectedNevera) ?? throw new ModelException("Nevera not found for IngredienteCompra.Nevera.") : null!;
            result.Add(child);
        }
        return result;
    }

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
