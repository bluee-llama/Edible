using NHibernate;
using Microsoft.Extensions.Logging;
using Edible.Application.Abstractions;
using Edible.Infrastructure.Exceptions;
using Edible.Infrastructure.Logging;
using Edible.Infrastructure.Repositories;
using Edible.Infrastructure.Security;

namespace Edible.Infrastructure;

public sealed class UnitOfWork : IUnitOfWork, IAsyncDisposable
{
    private readonly ISession _session;
    private readonly ILogger<UnitOfWork>? _logger;
    private readonly Microsoft.Extensions.Logging.ILoggerFactory? _loggerFactory;
    private ITransaction? _transaction;
    private SqlStatementCaptureScope? _sqlCaptureScope;
    private bool _disposed;

    public UnitOfWork(UnitOfWorkFactory unitOfWorkFactory)
        : this(
            unitOfWorkFactory.OpenSession(),
            unitOfWorkFactory.LoggerFactory,
            unitOfWorkFactory.PasswordHasher,
            unitOfWorkFactory.TokenGenerator)
    {
    }

    public UnitOfWork(ISession session, Microsoft.Extensions.Logging.ILoggerFactory? loggerFactory = null, IHariaPasswordHasher? passwordHasher = null, IHariaTokenGenerator? tokenGenerator = null)
    {
        _session = session;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory?.CreateLogger<UnitOfWork>();
        PasswordHasher = passwordHasher ?? new HariaPasswordHasher();
        TokenGenerator = tokenGenerator ?? new HariaTokenGenerator();
        UsuarioRepository = new UsuarioRepository(session, _loggerFactory?.CreateLogger<UsuarioRepository>());
        ChefRepository = new ChefRepository(session, _loggerFactory?.CreateLogger<ChefRepository>());
        NeveraRepository = new NeveraRepository(session, _loggerFactory?.CreateLogger<NeveraRepository>());
        ListaCompraRepository = new ListaCompraRepository(session, _loggerFactory?.CreateLogger<ListaCompraRepository>());
        RecetaRepository = new RecetaRepository(session, _loggerFactory?.CreateLogger<RecetaRepository>());
        LineaRecetaRepository = new LineaRecetaRepository(session, _loggerFactory?.CreateLogger<LineaRecetaRepository>());
        IngredienteRepository = new IngredienteRepository(session, _loggerFactory?.CreateLogger<IngredienteRepository>());
        IngredienteCompraRepository = new IngredienteCompraRepository(session, _loggerFactory?.CreateLogger<IngredienteCompraRepository>());
        IngredienteNeveraRepository = new IngredienteNeveraRepository(session, _loggerFactory?.CreateLogger<IngredienteNeveraRepository>());
        ComentarioRepository = new ComentarioRepository(session, _loggerFactory?.CreateLogger<ComentarioRepository>());
        PasoRepository = new PasoRepository(session, _loggerFactory?.CreateLogger<PasoRepository>());
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

    public Task BeginTransactionAsync(string? businessOperation = null)
    {
        try
        {
            if (_transaction is null)
            {
                var transactionId = Guid.NewGuid();
                var effectiveBusinessOperation = ResolveBusinessOperation(businessOperation);
                _transaction = _session.BeginTransaction();
                _sqlCaptureScope = SqlStatementCapture.Begin(transactionId, effectiveBusinessOperation);

                _logger?.LogDebug(
                    "NHibernate transaction started. BusinessOperation={BusinessOperation} TransactionId={TransactionId}",
                    effectiveBusinessOperation,
                    transactionId);
            }

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Error beginning NHibernate transaction.");
            DisposeSqlCaptureScope();
            throw new DataException("Error beginning NHibernate transaction.", ex);
        }
    }

    public async Task CommitAsync()
    {
        try
        {
            if (_transaction is not null)
            {
                await _transaction.CommitAsync();

                var sqlStatements = GetCapturedSqlStatements();
                var businessOperations = GetCapturedBusinessOperations();
                _logger?.LogInformation(
                    "NHibernate transaction committed successfully. BusinessOperation={BusinessOperation} BusinessOperations={BusinessOperations} TransactionId={TransactionId} SqlStatementCount={SqlStatementCount}",
                    _sqlCaptureScope?.BusinessOperation,
                    businessOperations,
                    _sqlCaptureScope?.TransactionId,
                    sqlStatements.Count);

                if (sqlStatements.Count > 0)
                {
                    _logger?.LogDebug(
                        "NHibernate SQL statements committed. BusinessOperation={BusinessOperation} BusinessOperations={BusinessOperations} TransactionId={TransactionId} SqlStatements={SqlStatements}",
                        _sqlCaptureScope?.BusinessOperation,
                        businessOperations,
                        _sqlCaptureScope?.TransactionId,
                        sqlStatements);
                }

                _transaction.Dispose();
                _transaction = null;
                DisposeSqlCaptureScope();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(
                ex,
                "Error committing NHibernate transaction. TransactionId={TransactionId}",
                _sqlCaptureScope?.TransactionId);
            throw new DataException("Error committing NHibernate transaction.", ex);
        }
    }

    public async Task RollbackAsync()
    {
        try
        {
            if (_transaction is not null)
            {
                await _transaction.RollbackAsync();

                var sqlStatements = GetCapturedSqlStatements();
                var businessOperations = GetCapturedBusinessOperations();
                _logger?.LogInformation(
                    "NHibernate transaction rolled back. BusinessOperation={BusinessOperation} BusinessOperations={BusinessOperations} TransactionId={TransactionId} SqlStatementCount={SqlStatementCount}",
                    _sqlCaptureScope?.BusinessOperation,
                    businessOperations,
                    _sqlCaptureScope?.TransactionId,
                    sqlStatements.Count);

                if (sqlStatements.Count > 0)
                {
                    _logger?.LogDebug(
                        "NHibernate SQL statements rolled back. BusinessOperation={BusinessOperation} BusinessOperations={BusinessOperations} TransactionId={TransactionId} SqlStatements={SqlStatements}",
                        _sqlCaptureScope?.BusinessOperation,
                        businessOperations,
                        _sqlCaptureScope?.TransactionId,
                        sqlStatements);
                }

                _transaction.Dispose();
                _transaction = null;
                DisposeSqlCaptureScope();
            }
        }
        catch (Exception ex)
        {
            _logger?.LogError(
                ex,
                "Error rolling back NHibernate transaction. TransactionId={TransactionId}",
                _sqlCaptureScope?.TransactionId);
            throw new DataException("Error rolling back NHibernate transaction.", ex);
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

        _session.Dispose();
        _disposed = true;
    }

    private IReadOnlyList<string> GetCapturedSqlStatements() =>
        _sqlCaptureScope?.GetStatements() ?? Array.Empty<string>();

    private IReadOnlyList<string> GetCapturedBusinessOperations() =>
        _sqlCaptureScope?.GetBusinessOperations() ?? Array.Empty<string>();

    private static string ResolveBusinessOperation(string? businessOperation)
    {
        if (!string.IsNullOrWhiteSpace(businessOperation))
        {
            return businessOperation;
        }

        return "UnitOfWork transaction";
    }

    private void DisposeSqlCaptureScope()
    {
        _sqlCaptureScope?.Dispose();
        _sqlCaptureScope = null;
    }
}
