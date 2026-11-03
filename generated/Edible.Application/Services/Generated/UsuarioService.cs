using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Application.Services;

public sealed partial class UsuarioService : ServiceEntity<Usuario>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUsuarioRepository _repository;
    private readonly IHariaPasswordHasher _passwordHasher;
    private readonly IHariaTokenGenerator _tokenGenerator;

    public UsuarioService(IUnitOfWork unitOfWork)
        : this(unitOfWork, ownsUnitOfWork: false)
    {
    }

    public UsuarioService(IUnitOfWorkFactory unitOfWorkFactory)
        : this(unitOfWorkFactory.Create(), ownsUnitOfWork: true)
    {
    }

    private UsuarioService(IUnitOfWork unitOfWork, bool ownsUnitOfWork) : base(unitOfWork, unitOfWork.UsuarioRepository, ownsUnitOfWork)
    {
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.UsuarioRepository;
        _passwordHasher = unitOfWork.PasswordHasher;
        _tokenGenerator = unitOfWork.TokenGenerator;
    }

    public Task<Usuario> AddUsuarioDefaultAsync(Usuario entity)
    {
        HashPasswordProperties(entity);
        return AddDefaultAsync(entity);
    }

    public Task DeleteUsuarioDefaultAsync(int id) =>
        DeleteDefaultAsync(id);

    public Task<Usuario?> ReadOIDUsuarioDefaultAsync(int id) =>
        ReadOIDDefaultAsync(id);

    public Task<IReadOnlyList<Usuario>> ReadAllUsuarioDefaultAsync() =>
        ReadAllDefaultAsync();

    public Task<Usuario> ModifierUsuarioDefaultAsync(Usuario entity)
    {
        HashPasswordProperties(entity);
        return ModifierDefaultAsync(entity);
    }

    private void HashPasswordProperties(Usuario entity)
    {
        if (entity.Password is not null)
        {
            entity.Password = _passwordHasher.HashPasswordIfNeeded(entity.Password);
        }
    }

    public async Task<Usuario> NewAsync(string p_name, string p_password, bool p_publico, DietaEspecial p_dieta, Nevera p_nevera, ListaCompra p_listaCompra)
    {
        Usuario entity = new Usuario
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
        Usuario entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Usuario not found.");
        await _repository.DeleteAsync(entity);
    }

    public async Task ModifyAsync(int p_oid, string p_name, string p_password, bool p_publico, DietaEspecial p_dieta, Nevera p_nevera, ListaCompra p_listaCompra)
    {
        Usuario entity = await _repository.ReadAsync(p_oid)
            ?? throw new ModelException("Usuario not found.");
        entity.Name = p_name;
        entity.Password = _passwordHasher.HashPasswordIfNeeded(p_password);
        entity.Publico = p_publico;
        entity.Dieta = p_dieta;
        await _repository.SaveAsync(entity);
    }

    public async Task<string?> LoginAsync(int p_oid, string p_password)
    {
        try
        {
            Usuario? entity = await _repository.ReadAsync(p_oid);
            if (entity is null)
            {
                return null;
            }

            if (!_passwordHasher.VerifyPassword(entity.Password, p_password))
            {
                return null;
            }

            var tokenSubject = Convert.ToString(entity.Id, System.Globalization.CultureInfo.InvariantCulture) ?? string.Empty;
            var tokenClaims = new Dictionary<string, string?>(StringComparer.Ordinal)
            {
                ["id"] = Convert.ToString(entity.Id, System.Globalization.CultureInfo.InvariantCulture),
            };
            return _tokenGenerator.CreateToken(tokenSubject, tokenClaims);
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<Usuario?> ReadOIDAsync(int p_oid)
    {
        return await _repository.ReadAsync(p_oid);
    }

    public async Task<IReadOnlyList<Usuario>> ReadAllAsync()
    {
        return await _repository.ReadAllAsync();
    }

}
