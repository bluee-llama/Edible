namespace Edible.Application.Abstractions;

public interface IUnitOfWork : IAsyncDisposable
{
    IUsuarioRepository UsuarioRepository { get; }
    IChefRepository ChefRepository { get; }
    INeveraRepository NeveraRepository { get; }
    IListaCompraRepository ListaCompraRepository { get; }
    IRecetaRepository RecetaRepository { get; }
    ILineaRecetaRepository LineaRecetaRepository { get; }
    IIngredienteRepository IngredienteRepository { get; }
    IIngredienteCompraRepository IngredienteCompraRepository { get; }
    IIngredienteNeveraRepository IngredienteNeveraRepository { get; }
    IComentarioRepository ComentarioRepository { get; }
    IPasoRepository PasoRepository { get; }

    IHariaPasswordHasher PasswordHasher { get; }
    IHariaTokenGenerator TokenGenerator { get; }

    Task BeginTransactionAsync(string? businessOperation = null);
    Task CommitAsync();
    Task RollbackAsync();
}
