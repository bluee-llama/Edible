using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Edible.Application.Abstractions;
using Edible.Infrastructure.Data;
using Edible.Infrastructure.Exceptions;
using Edible.Infrastructure.Repositories;
using Edible.Infrastructure.Security;

namespace Edible.Infrastructure;

public sealed class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly EdibleDbContext _dbContext;
    private readonly ILogger<UnitOfWork>? _logger;
    private readonly ILoggerFactory? _loggerFactory;
    private IDbContextTransaction? _transaction;
    private bool _disposed;

    public UnitOfWork(UnitOfWorkFactory unitOfWorkFactory)
        : this(
            unitOfWorkFactory.CreateDbContext(),
            unitOfWorkFactory.LoggerFactory,
            unitOfWorkFactory.PasswordHasher,
            unitOfWorkFactory.TokenGenerator)
    {
    }

    public UnitOfWork(EdibleDbContext dbContext, ILoggerFactory? loggerFactory = null, IHariaPasswordHasher? passwordHasher = null, IHariaTokenGenerator? tokenGenerator = null)
    {
        _dbContext = dbContext;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<UnitOfWork>();
        PasswordHasher = passwordHasher ?? new HariaPasswordHasher();
        TokenGenerator = tokenGenerator ?? new HariaTokenGenerator();
        UsuarioRepository = new UsuarioRepository(dbContext, _loggerFactory?.CreateLogger<UsuarioRepository>());
        ChefRepository = new ChefRepository(dbContext, _loggerFactory?.CreateLogger<ChefRepository>());
        NeveraRepository = new NeveraRepository(dbContext, _loggerFactory?.CreateLogger<NeveraRepository>());
        ListaCompraRepository = new ListaCompraRepository(dbContext, _loggerFactory?.CreateLogger<ListaCompraRepository>());
        RecetaRepository = new RecetaRepository(dbContext, _loggerFactory?.CreateLogger<RecetaRepository>());
        LineaRecetaRepository = new LineaRecetaRepository(dbContext, _loggerFactory?.CreateLogger<LineaRecetaRepository>());
        IngredienteRepository = new IngredienteRepository(dbContext, _loggerFactory?.CreateLogger<IngredienteRepository>());
        IngredienteCompraRepository = new IngredienteCompraRepository(dbContext, _loggerFactory?.CreateLogger<IngredienteCompraRepository>());
        IngredienteNeveraRepository = new IngredienteNeveraRepository(dbContext, _loggerFactory?.CreateLogger<IngredienteNeveraRepository>());
        ComentarioRepository = new ComentarioRepository(dbContext, _loggerFactory?.CreateLogger<ComentarioRepository>());
        PasoRepository = new PasoRepository(dbContext, _loggerFactory?.CreateLogger<PasoRepository>());
    }

    public IUsuarioRepository UsuarioRepository { get; }
    public IChefRepository ChefRepository { get; }
    public INeveraRepository NeveraRepository { get; }
    public IListaCompraRepository ListaCompraRepository { get; }
    public IRecetaRepository RecetaRepository { get; }
    public ILineaRecetaRepository LineaRecetaRepository { get; }
    public IIngredienteRepository IngredienteRepository { get; }
    public IIngredienteCompraRepository IngredienteCompraRepository { get; }
    public IIngredienteNeveraRepository IngredienteNeveraRepository { get; }
    public IComentarioRepository ComentarioRepository { get; }
    public IPasoRepository PasoRepository { get; }

    public IHariaPasswordHasher PasswordHasher { get; }

    public IHariaTokenGenerator TokenGenerator { get; }

    public async Task BeginTransactionAsync(string? businessOperation = null)
    {
        try
        {
            _transaction ??= await _dbContext.Database.BeginTransactionAsync();
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error beginning Entity Framework transaction.");
            throw new DataException("Error beginning Entity Framework transaction.", ex);
        }
    }

    public async Task CommitAsync()
    {
        try
        {
            await _dbContext.SaveChangesAsync();
            if (_transaction is not null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error committing Entity Framework transaction.");
            throw new DataException("Error committing Entity Framework transaction.", ex);
        }
    }

    public async Task RollbackAsync()
    {
        try
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error rolling back Entity Framework transaction.");
            throw new DataException("Error rolling back Entity Framework transaction.", ex);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        if (_transaction is not null)
        {
            await RollbackAsync();
        }

        await _dbContext.DisposeAsync();
        _disposed = true;
    }
}
