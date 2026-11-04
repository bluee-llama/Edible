using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Application.UseCases.Comentario;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Infrastructure;

namespace Edible.Tests.Manual;

public class ComentarioTests
{
    private sealed class Datos
    {
        public int UsuarioId { get; init; }
        public int RecetaId { get; init; }
        public int OtraRecetaId { get; init; }
    }

    private static async Task<Datos> PrepararAsync(UnitOfWork uow)
    {
        var chef = await RecetaScenarioHelper.CrearChefAsync(uow, "chef");
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(uow, "comentador");
        var huevos = await RecetaScenarioHelper.CrearIngredienteAsync(uow, "Huevos");
        var r1 = await RecetaScenarioHelper.CrearRecetaAsync(uow, chef, "Tortilla", DietaEspecial.ninguna, 0f, 30, 1, huevos);
        var r2 = await RecetaScenarioHelper.CrearRecetaAsync(uow, chef, "Revuelto", DietaEspecial.ninguna, 0f, 10, 1, huevos);
        return new Datos { UsuarioId = usuario.Id, RecetaId = r1.Id, OtraRecetaId = r2.Id };
    }

    [Fact]
    public async Task RecalcularValoracionMedia_PrimerComentario_MediaIgualALaValoracion()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var d = await PrepararAsync(scenario.UnitOfWork);
        var useCase = new ComentarioUseCase(scenario.UnitOfWork);

        var comentario = await useCase.RecalcularValoracionMediaAsync(1, "Muy rica", 4, d.UsuarioId, d.RecetaId);

        Assert.NotEqual(0, comentario.Id);
        scenario.Session.Clear(); // releer desde BD, no de la caché de sesión
        var receta = await scenario.UnitOfWork.RecetaRepository.ReadAsync(d.RecetaId);
        Assert.Equal(4.0, receta!.ValoracionMedia, 2);
        Assert.Single(await scenario.UnitOfWork.ComentarioRepository.ReadAllAsync());
    }

    [Fact]
    public async Task RecalcularValoracionMedia_VariosComentarios_PersisteMediaYIgnoraOtrasRecetas()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var d = await PrepararAsync(scenario.UnitOfWork);
        var useCase = new ComentarioUseCase(scenario.UnitOfWork);

        await useCase.RecalcularValoracionMediaAsync(1, "a", 4, d.UsuarioId, d.RecetaId);
        await useCase.RecalcularValoracionMediaAsync(1, "otra receta", 5, d.UsuarioId, d.OtraRecetaId);
        await useCase.RecalcularValoracionMediaAsync(1, "b", 2, d.UsuarioId, d.RecetaId);

        scenario.Session.Clear();
        var receta = await scenario.UnitOfWork.RecetaRepository.ReadAsync(d.RecetaId);
        var otra = await scenario.UnitOfWork.RecetaRepository.ReadAsync(d.OtraRecetaId);
        Assert.Equal(3.0, receta!.ValoracionMedia, 2); // (4 + 2) / 2
        Assert.Equal(5.0, otra!.ValoracionMedia, 2);
    }

    [Fact]
    public async Task RecalcularValoracionMedia_RecetaInexistente_LanzaModelExceptionYNoGuarda()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var d = await PrepararAsync(scenario.UnitOfWork);
        var useCase = new ComentarioUseCase(scenario.UnitOfWork);

        await Assert.ThrowsAsync<ModelException>(() =>
            useCase.RecalcularValoracionMediaAsync(1, "x", 3, d.UsuarioId, 99999));

        scenario.Session.Clear();
        Assert.Empty(await scenario.UnitOfWork.ComentarioRepository.ReadAllAsync());
    }

    [Fact]
    public async Task RecalcularValoracionMedia_UsuarioInexistente_LanzaModelExceptionYNoGuarda()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var d = await PrepararAsync(scenario.UnitOfWork);
        var useCase = new ComentarioUseCase(scenario.UnitOfWork);

        await Assert.ThrowsAsync<ModelException>(() =>
            useCase.RecalcularValoracionMediaAsync(1, "x", 3, 99999, d.RecetaId));

        scenario.Session.Clear();
        Assert.Empty(await scenario.UnitOfWork.ComentarioRepository.ReadAllAsync());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public async Task RecalcularValoracionMedia_ValoracionFueraDeRango_LanzaModelException(int valoracion)
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var d = await PrepararAsync(scenario.UnitOfWork);
        var useCase = new ComentarioUseCase(scenario.UnitOfWork);

        await Assert.ThrowsAsync<ModelException>(() =>
            useCase.RecalcularValoracionMediaAsync(1, "x", valoracion, d.UsuarioId, d.RecetaId));
    }

    [Fact]
    public async Task RecalcularValoracionMedia_TituloVacio_LanzaModelException()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var d = await PrepararAsync(scenario.UnitOfWork);
        var useCase = new ComentarioUseCase(scenario.UnitOfWork);

        await Assert.ThrowsAsync<ModelException>(() =>
            useCase.RecalcularValoracionMediaAsync(1, "  ", 3, d.UsuarioId, d.RecetaId));
    }

    [Fact]
    public async Task RecalcularValoracionMedia_FallaTrasInsertarComentario_HaceRollback()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var d = await PrepararAsync(scenario.UnitOfWork);
        var useCase = new ComentarioUseCase(new UnitOfWorkConComentarioQueFalla(scenario.UnitOfWork));

        await Assert.ThrowsAsync<ModelException>(() =>
            useCase.RecalcularValoracionMediaAsync(1, "x", 5, d.UsuarioId, d.RecetaId));

        // Tras el rollback no debe quedar ni el comentario ni el cambio en la receta
        scenario.Session.Clear();
        Assert.Empty(await scenario.UnitOfWork.ComentarioRepository.ReadAllAsync());
        var receta = await scenario.UnitOfWork.RecetaRepository.ReadAsync(d.RecetaId);
        Assert.Equal(0.0, receta!.ValoracionMedia, 2);
    }

    // ---- Dobles de prueba para simular el fallo ----

    // Ejecuta la consulta real (lo que fuerza el INSERT del comentario en BD) y DESPUÉS falla.
    private sealed class ComentarioRepositoryQueFalla : IComentarioRepository
    {
        private readonly IComentarioRepository _inner;
        public ComentarioRepositoryQueFalla(IComentarioRepository inner) => _inner = inner;

        public async Task<IReadOnlyList<Comentario>> ReadAllAsync()
        {
            await _inner.ReadAllAsync();
            throw new InvalidOperationException("Fallo simulado tras insertar el comentario.");
        }

        public Task<Comentario?> ReadAsync(object id) => _inner.ReadAsync(id);
        public Task SaveAsync(Comentario entity) => _inner.SaveAsync(entity);
        public Task DeleteAsync(Comentario entity) => _inner.DeleteAsync(entity);
    }

    private sealed class UnitOfWorkConComentarioQueFalla : IUnitOfWork
    {
        private readonly IUnitOfWork _inner;

        public UnitOfWorkConComentarioQueFalla(IUnitOfWork inner)
        {
            _inner = inner;
            ComentarioRepository = new ComentarioRepositoryQueFalla(inner.ComentarioRepository);
        }

        public IUsuarioRepository UsuarioRepository => _inner.UsuarioRepository;
        public IChefRepository ChefRepository => _inner.ChefRepository;
        public INeveraRepository NeveraRepository => _inner.NeveraRepository;
        public IListaCompraRepository ListaCompraRepository => _inner.ListaCompraRepository;
        public IRecetaRepository RecetaRepository => _inner.RecetaRepository;
        public ILineaRecetaRepository LineaRecetaRepository => _inner.LineaRecetaRepository;
        public IIngredienteRepository IngredienteRepository => _inner.IngredienteRepository;
        public IIngredienteCompraRepository IngredienteCompraRepository => _inner.IngredienteCompraRepository;
        public IIngredienteNeveraRepository IngredienteNeveraRepository => _inner.IngredienteNeveraRepository;
        public IComentarioRepository ComentarioRepository { get; }
        public IPasoRepository PasoRepository => _inner.PasoRepository;
        public IHariaPasswordHasher PasswordHasher => _inner.PasswordHasher;
        public IHariaTokenGenerator TokenGenerator => _inner.TokenGenerator;

        public Task BeginTransactionAsync(string? businessOperation = null) => _inner.BeginTransactionAsync(businessOperation);
        public Task CommitAsync() => _inner.CommitAsync();
        public Task RollbackAsync() => _inner.RollbackAsync();
        public ValueTask DisposeAsync() => ValueTask.CompletedTask; // no cerramos la sesión del escenario
    }
}