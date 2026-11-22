using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Edible.Application.Exceptions;
using Edible.Application.UseCases.Nevera;
using Edible.Application.Abstractions;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
namespace Edible.Tests.Manual;
public class NeveraTests
{
     [Fact]
    public async Task RecorrerListaCompra_ItemYaExisteEnNevera_SumaCantidadYBorraDeListaCompra()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "UsuarioFusionStock");

        var nevera = usuario.Nevera!;
        var listaCompra = usuario.ListaCompra!;
        var ingLeche = new Ingrediente
        {
            Nombre = "Leche Desnatada",
            Tipo = TipoIngrediente.leche_y_productos_lacteos
        };
        await scenario.UnitOfWork.IngredienteRepository.SaveAsync(ingLeche);

        var stockPrevio = new IngredienteNevera
        {
            Cantidad = 2,
            FechaCaducidad = DateTime.UtcNow.AddDays(5),
            IngredienteCatalogo = ingLeche,
            Nevera = nevera
        };
        await scenario.UnitOfWork.IngredienteNeveraRepository.SaveAsync(stockPrevio);

        var itemComprado = new IngredienteCompra
        {
            Cantidad = 3,
            Comprado = true,
            IngredienteCatalogo = ingLeche,
            ListaCompra = listaCompra,
            Nevera = nevera
        };
        await scenario.UnitOfWork.IngredienteCompraRepository.SaveAsync(itemComprado);
        var useCase = new NeveraUseCase(scenario.UnitOfWork);

        await useCase.RecorrerListaCompraAsync(usuario.Id);
        scenario.Session.Clear();
        
        var todosEnNevera = await scenario.UnitOfWork.IngredienteNeveraRepository.ReadAllAsync();
        var enMiNevera = todosEnNevera.Where(i => i.Nevera.Id == nevera.Id).ToList();
        var lecheFinal = Assert.Single(enMiNevera);
        Assert.Equal(5, lecheFinal.Cantidad);

        var todosEnCompra = await scenario.UnitOfWork.IngredienteCompraRepository.ReadAllAsync();
        var enMiLista = todosEnCompra.Where(i => i.ListaCompra.Id == listaCompra.Id).ToList();
        Assert.Empty(enMiLista);
    }

    [Fact]
    public async Task FiltrarNevera_FiltraPorEstadoCaducidadYTipo_DevuelveSoloCoincidentes()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "UsuarioFiltro");
        var nevera = usuario.Nevera!;

        var ingLeche = new Ingrediente { Nombre = "Leche", Tipo = TipoIngrediente.leche_y_productos_lacteos };
        var ingYogur = new Ingrediente { Nombre = "Yogur", Tipo = TipoIngrediente.leche_y_productos_lacteos };
        var ingManzana = new Ingrediente { Nombre = "Manzana", Tipo = TipoIngrediente.frutas_y_productos_a_base_de_fruta };

        await scenario.UnitOfWork.IngredienteRepository.SaveAsync(ingLeche);
        await scenario.UnitOfWork.IngredienteRepository.SaveAsync(ingYogur);
        await scenario.UnitOfWork.IngredienteRepository.SaveAsync(ingManzana);

        var hoy = new DateTime(2026, 5, 10);
        var limiteCerca = hoy.AddDays(3);

        var itemCaducado = new IngredienteNevera
        {
            Cantidad = 1,
            FechaCaducidad = hoy.AddDays(-2),
            IngredienteCatalogo = ingLeche,
            Nevera = nevera
        };

        var itemCerca = new IngredienteNevera
        {
            Cantidad = 4,
            FechaCaducidad = hoy.AddDays(2),
            IngredienteCatalogo = ingYogur,
            Nevera = nevera
        };

        var itemPerfecto = new IngredienteNevera
        {
            Cantidad = 6,
            FechaCaducidad = hoy.AddDays(10),
            IngredienteCatalogo = ingManzana,
            Nevera = nevera
        };

        await scenario.UnitOfWork.IngredienteNeveraRepository.SaveAsync(itemCaducado);
        await scenario.UnitOfWork.IngredienteNeveraRepository.SaveAsync(itemCerca);
        await scenario.UnitOfWork.IngredienteNeveraRepository.SaveAsync(itemPerfecto);
        
        // Filtrar caducados -> solo debe salir la leche
        var caducados = await scenario.UnitOfWork.IngredienteNeveraRepository.FiltrarNeveraAsync(
            nevera.Id, TipoIngrediente.ninguno, limiteCerca, hoy, EstadoCaducidad.caducado);
        var unicoCaducado = Assert.Single(caducados);
        Assert.Equal("Leche", unicoCaducado.IngredienteCatalogo.Nombre);

        // Filtrar cerca de caducar -> solo debe salir el yogur
        var cerca = await scenario.UnitOfWork.IngredienteNeveraRepository.FiltrarNeveraAsync(
            nevera.Id, TipoIngrediente.ninguno, limiteCerca, hoy, EstadoCaducidad.cerca);
        var unicoCerca = Assert.Single(cerca);
        Assert.Equal("Yogur", unicoCerca.IngredienteCatalogo.Nombre);

        // Filtrar perfectos y de tipo fruta -> solo debe salir la manzana
        var frutasPerfectas = await scenario.UnitOfWork.IngredienteNeveraRepository.FiltrarNeveraAsync(
            nevera.Id, TipoIngrediente.frutas_y_productos_a_base_de_fruta, limiteCerca, hoy, EstadoCaducidad.perfecto);
        var unicaFruta = Assert.Single(frutasPerfectas);
        Assert.Equal("Manzana", unicaFruta.IngredienteCatalogo.Nombre);

        // Filtrar perfectos pero de tipo lácteo -> debe devolver 0 (no hay lácteos perfectos)
        var lacteosPerfectos = await scenario.UnitOfWork.IngredienteNeveraRepository.FiltrarNeveraAsync(
            nevera.Id, TipoIngrediente.leche_y_productos_lacteos, limiteCerca, hoy, EstadoCaducidad.perfecto);
        Assert.Empty(lacteosPerfectos);
    }

     [Fact]
    public async Task FiltrarNevera_NeveraVacia_DevuelveListaVacia()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "UsuarioNeveraVacia");

        var nevera = usuario.Nevera!;
        var hoy = new DateTime(2026, 5, 10);
        var limite = hoy.AddDays(3);

        // Consultar una nevera sin ingredientes
        var resultado = await scenario.UnitOfWork.IngredienteNeveraRepository.FiltrarNeveraAsync(nevera.Id, TipoIngrediente.ninguno, limite, hoy, EstadoCaducidad.ninguno);

        Assert.NotNull(resultado);
        Assert.Empty(resultado);
    }

    [Fact]
    public async Task RecorrerListaCompra_ItemsComprados_MueveANeveraYBorraDeListaCompra()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();

        // Crear usuario con su nevera y lista de la compra asociadas usando el helper del equipo
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "Usuario1");
        var nevera = usuario.Nevera!;
        var listaCompra = usuario.ListaCompra!;

        // Crear ingredientes del catalogo y de la lista de la compra
        var ingLeche = new Ingrediente
        {
            Nombre = "Leche Entera",
            Tipo = TipoIngrediente.leche_y_productos_lacteos
        };
        var ingPan = new Ingrediente
        {
            Nombre = "Pan de Molde",
            Tipo = TipoIngrediente.cereales_y_productos_a_base_de_cereales
        };

        await scenario.UnitOfWork.IngredienteRepository.SaveAsync(ingLeche);
        await scenario.UnitOfWork.IngredienteRepository.SaveAsync(ingPan);

        // Ingrediente comprado
        var ingredienteComprado = new IngredienteCompra
        {
            Cantidad = 2,
            Comprado = true,
            IngredienteCatalogo = ingLeche,
            ListaCompra = listaCompra,
            Nevera = nevera
        };

        var ingredienteNoComprado = new IngredienteCompra
        {
            Cantidad = 1,
            Comprado = false,
            IngredienteCatalogo = ingPan,
            ListaCompra = listaCompra,
            Nevera = nevera
        };

        await scenario.UnitOfWork.IngredienteCompraRepository.SaveAsync(ingredienteComprado);
        await scenario.UnitOfWork.IngredienteCompraRepository.SaveAsync(ingredienteNoComprado);

        var useCase = new NeveraUseCase(scenario.UnitOfWork);

        // Ejecutar la transacción de recorrer la lista de la compra y añadir a la nevera
        var neveraResultado = await useCase.RecorrerListaCompraAsync(usuario.Id);

        scenario.Session.Clear();

        // Comprobar que los datos han cambiado correctamente
        Assert.NotNull(neveraResultado);

        // Verificar nevera
        var todosEnNevera = await scenario.UnitOfWork.IngredienteNeveraRepository.ReadAllAsync();
        var enMiNevera = todosEnNevera.Where(i => i.Nevera.Id == nevera.Id).ToList();
        var lecheEnNevera = Assert.Single(enMiNevera);
        Assert.Equal(ingLeche.Id, lecheEnNevera.IngredienteCatalogo.Id);
        Assert.Equal(2, lecheEnNevera.Cantidad);

        // Verificar lista de la compra
        var todosEnCompra = await scenario.UnitOfWork.IngredienteCompraRepository.ReadAllAsync();
        var enMiLista = todosEnCompra.Where(i => i.ListaCompra.Id == listaCompra.Id).ToList();
        var panEnCompra = Assert.Single(enMiLista);
        Assert.Equal(ingPan.Id, panEnCompra.IngredienteCatalogo.Id);
        Assert.Equal(1, panEnCompra.Cantidad);
    }

    [Fact]
    public async Task RecorrerListaCompra_SinItemsComprados_LanzaModelException()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();

        // Crear usuario con lista donde ningún ingrediente está marcado como comprado
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "UsuarioSinComprados");
        var nevera = usuario.Nevera!;
        var listaCompra = usuario.ListaCompra!;

        var ingManzana = new Ingrediente
        {
            Nombre = "Manzana",
            Tipo = TipoIngrediente.frutas_y_productos_a_base_de_fruta
        };
        await scenario.UnitOfWork.IngredienteRepository.SaveAsync(ingManzana);

        var itemPendiente = new IngredienteCompra
        {
            Cantidad = 3,
            Comprado = false,
            IngredienteCatalogo = ingManzana,
            ListaCompra = listaCompra,
            Nevera = nevera
        };
        await scenario.UnitOfWork.IngredienteCompraRepository.SaveAsync(itemPendiente);
        var useCase = new NeveraUseCase(scenario.UnitOfWork);
        
        await Assert.ThrowsAsync<ModelException>(() => useCase.RecorrerListaCompraAsync(usuario.Id));
        scenario.Session.Clear();

        // Verificar que no se ha añadido nada a la nevera
        var todosEnNevera = await scenario.UnitOfWork.IngredienteNeveraRepository.ReadAllAsync();
        var enMiNevera = todosEnNevera.Where(i => i.Nevera.Id == nevera.Id).ToList();
        Assert.Empty(enMiNevera);

        // Verificar que el ingrediente sigue intacto en la lista de compra
        var todosEnCompra = await scenario.UnitOfWork.IngredienteCompraRepository.ReadAllAsync();
        var enMiLista = todosEnCompra.Where(i => i.ListaCompra.Id == listaCompra.Id).ToList();
        var manzanaEnCompra = Assert.Single(enMiLista);
        Assert.Equal(3, manzanaEnCompra.Cantidad);
        Assert.False(manzanaEnCompra.Comprado);
    }

     [Fact]
    public async Task RecorrerListaCompra_FallaGuardadoEnNevera_HaceRollbackYNoBorraDeLista()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "UsuarioRollback");
        var nevera = usuario.Nevera!;
        var listaCompra = usuario.ListaCompra!;

        var ingQueso = new Ingrediente
        {
            Nombre = "Queso Manchego",
            Tipo = TipoIngrediente.leche_y_productos_lacteos
        };
        await scenario.UnitOfWork.IngredienteRepository.SaveAsync(ingQueso);
        var itemComprado = new IngredienteCompra
        {
            Cantidad = 2,
            Comprado = true,
            IngredienteCatalogo = ingQueso,
            ListaCompra = listaCompra,
            Nevera = nevera
        };
        await scenario.UnitOfWork.IngredienteCompraRepository.SaveAsync(itemComprado);

        var uowFalla = new UnitOfWorkConNeveraQueFalla(scenario.UnitOfWork);
        var useCase = new NeveraUseCase(uowFalla);

        // Debe fallar y lanzar ModelException
        await Assert.ThrowsAsync<ModelException>(() => useCase.RecorrerListaCompraAsync(usuario.Id));
        scenario.Session.Clear();

        // Tras el Rollback, la nevera debe seguir vacía y el ingrediente debe seguir en la lista de compra
        var todosEnNevera = await scenario.UnitOfWork.IngredienteNeveraRepository.ReadAllAsync();
        var enMiNevera = todosEnNevera.Where(i => i.Nevera.Id == nevera.Id).ToList();
        Assert.Empty(enMiNevera);

        var todosEnCompra = await scenario.UnitOfWork.IngredienteCompraRepository.ReadAllAsync();
        var enMiLista = todosEnCompra.Where(i => i.ListaCompra.Id == listaCompra.Id).ToList();
        var quesoEnCompra = Assert.Single(enMiLista);
        Assert.Equal(2, quesoEnCompra.Cantidad);
        Assert.True(quesoEnCompra.Comprado);
    }

    private sealed class IngredienteNeveraRepositoryQueFalla : IIngredienteNeveraRepository
    {
        private readonly IIngredienteNeveraRepository _inner;
        public IngredienteNeveraRepositoryQueFalla(IIngredienteNeveraRepository inner) => _inner = inner;
        public Task<IReadOnlyList<IngredienteNevera>> ReadAllAsync() => _inner.ReadAllAsync();
        public Task<IngredienteNevera?> ReadAsync(object id) => _inner.ReadAsync(id);
        public Task DeleteAsync(IngredienteNevera entity) => _inner.DeleteAsync(entity);
        public Task<IReadOnlyList<IngredienteNevera>> FiltrarNeveraAsync(int p_nevera_oid, TipoIngrediente p_tipo, DateTime p_fechaLimiteCerca, DateTime p_fechaActual, EstadoCaducidad p_estado)
            => _inner.FiltrarNeveraAsync(p_nevera_oid, p_tipo, p_fechaLimiteCerca, p_fechaActual, p_estado);
        public Task SaveAsync(IngredienteNevera entity)
        {
            throw new InvalidOperationException("Fallo simulado al guardar en la nevera.");
        }
    }
    private sealed class UnitOfWorkConNeveraQueFalla : IUnitOfWork
    {
        private readonly IUnitOfWork _inner;
        public UnitOfWorkConNeveraQueFalla(IUnitOfWork inner)
        {
            _inner = inner;
            IngredienteNeveraRepository = new IngredienteNeveraRepositoryQueFalla(inner.IngredienteNeveraRepository);
        }
        public IUsuarioRepository UsuarioRepository => _inner.UsuarioRepository;
        public IChefRepository ChefRepository => _inner.ChefRepository;
        public INeveraRepository NeveraRepository => _inner.NeveraRepository;
        public IListaCompraRepository ListaCompraRepository => _inner.ListaCompraRepository;
        public IRecetaRepository RecetaRepository => _inner.RecetaRepository;
        public ILineaRecetaRepository LineaRecetaRepository => _inner.LineaRecetaRepository;
        public IIngredienteRepository IngredienteRepository => _inner.IngredienteRepository;
        public IIngredienteCompraRepository IngredienteCompraRepository => _inner.IngredienteCompraRepository;
        public IIngredienteNeveraRepository IngredienteNeveraRepository { get; }
        public IComentarioRepository ComentarioRepository => _inner.ComentarioRepository;
        public IPasoRepository PasoRepository => _inner.PasoRepository;
        public IHariaPasswordHasher PasswordHasher => _inner.PasswordHasher;
        public IHariaTokenGenerator TokenGenerator => _inner.TokenGenerator;
        public Task BeginTransactionAsync(string? businessOperation = null) => _inner.BeginTransactionAsync(businessOperation);
        public Task CommitAsync() => _inner.CommitAsync();
        public Task RollbackAsync() => _inner.RollbackAsync();
        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
