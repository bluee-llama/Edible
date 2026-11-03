using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Application.Services;

public sealed partial class ComentarioService : ServiceEntity<Comentario>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IComentarioRepository _repository;

    public ComentarioService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public ComentarioService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private ComentarioService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.ComentarioRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.ComentarioRepository;
    }

    public Task<Comentario> AddComentarioDefaultAsync(Comentario entity) =>
        AddDefaultAsync(entity);

    public Task DeleteComentarioDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Comentario?> ReadOIDComentarioDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Comentario>> ReadAllComentarioDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Comentario> ModifierComentarioDefaultAsync(Comentario entity) =>
        ModifierDefaultAsync(entity);

    public async Task<Comentario> NewAsync(int p_user_id, string p_titulo, int p_valoracion, int p_comentador_oid, int p_receta_oid)
    {
        Usuario comentadorEntity = await _unitOfWork.UsuarioRepository.ReadAsync(p_comentador_oid)
            ?? throw new ModelException("Usuario not found for Comentario.Comentador.");

        Receta recetaEntity = await _unitOfWork.RecetaRepository.ReadAsync(p_receta_oid)
            ?? throw new ModelException("Receta not found for Comentario.Receta.");

        Comentario entity = new Comentario
        {
            User_id = p_user_id,
            Titulo = p_titulo,
            Valoracion = p_valoracion,
            Comentador = comentadorEntity,
            Receta = recetaEntity,
        };

        await _repository.SaveAsync(entity);
        return entity;
    }

    public async Task DestroyAsync(int p_oid)
    {
        Comentario entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Comentario not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, int p_user_id, string p_titulo, int p_valoracion)
    {
        Comentario entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Comentario not found.");
        entity.User_id = p_user_id;
        entity.Titulo = p_titulo;
        entity.Valoracion = p_valoracion;
        await _repository.SaveAsync(entity);
    }

    public async Task<Comentario?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Comentario>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
