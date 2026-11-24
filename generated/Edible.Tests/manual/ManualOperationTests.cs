using Edible.Application.Services;
using Edible.Application.UseCases.Comentario;
using Edible.Application.UseCases.Nevera;
using Edible.Application.UseCases.Receta;
using Edible.Domain.Entities;
using Xunit;
using Xunit.Abstractions;
using Edible.Domain.Enums;

/*PROTECTED REGION ID(ManualOperationTests_Imports) ENABLED START*/
using System;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Edible.Application.Exceptions;
using Edible.Infrastructure;
using Edible.Infrastructure.Data;

// Cada test crea/borra el esquema: no ejecutar en paralelo.
[assembly: CollectionBehavior(DisableTestParallelization = true)]
/*PROTECTED REGION END*/

namespace Edible.Tests.Manual;

/*PROTECTED REGION ID(ManualOperationTests_Fixture) ENABLED START*/
public class Scenario : IAsyncDisposable
{
    public SqliteConnection Connection { get; }
    public DbContextOptions<EdibleDbContext> Options { get; }
    public EdibleDbContext DbContext { get; }
    public ChangeTracker Session => DbContext.ChangeTracker;
    public UnitOfWork UnitOfWork { get; }

    public Scenario(
        SqliteConnection connection,
        DbContextOptions<EdibleDbContext> options,
        EdibleDbContext dbContext,
        UnitOfWork unitOfWork)
    {
        Connection = connection;
        Options = options;
        DbContext = dbContext;
        UnitOfWork = unitOfWork;
    }

    public UnitOfWork CreateFreshUnitOfWork() => new(new EdibleDbContext(Options));

    public async ValueTask DisposeAsync()
    {
        await UnitOfWork.DisposeAsync();
        await Connection.DisposeAsync();
    }
}

public static class TestFixture
{
    public static async Task<Scenario> CreateScenarioAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<EdibleDbContext>()
            .UseSqlite(connection)
            .Options;

        await using (var schemaDb = new EdibleDbContext(options))
        {
            await schemaDb.Database.EnsureCreatedAsync();
        }

        var dbContext = new EdibleDbContext(options);
        var unitOfWork = new UnitOfWork(dbContext);

        return new Scenario(connection, options, dbContext, unitOfWork);
    }
}
/*PROTECTED REGION END*/

public sealed class ManualOperationTests
{
    private readonly ITestOutputHelper _output;

    public ManualOperationTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task Usuario_PonerPublico_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:Usuario:ponerPublico:arrange) ENABLED START*/
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var usuarioService = new UsuarioService(scenario.UnitOfWork);
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "UsuarioPublico");
        usuario.Publico = false;
        await scenario.UnitOfWork.UsuarioRepository.SaveAsync(usuario);
        int usuarioId = usuario.Id;
        /*PROTECTED REGION END*/

        var operationName = "ponerPublico";
        var operationType = "Custom";
        var executeManualInvocation = true;
        UsuarioService? service = usuarioService;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Usuario:ponerPublico:invoke) ENABLED START*/
        await service!.PonerPublicoAsync(usuarioId);
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Usuario:ponerPublico:assert) ENABLED START*/
        await using (var fresh = scenario.CreateFreshUnitOfWork())
        {
            var usuarioBD = await fresh.UsuarioRepository.ReadAsync(usuarioId);
            Assert.NotNull(usuarioBD);
            Assert.True(usuarioBD!.Publico);
        }

        // Ya es público: una segunda llamada debe rechazarse con ModelException.
        await Assert.ThrowsAsync<ModelException>(() => service!.PonerPublicoAsync(usuarioId));
        // Un usuario inexistente también se rechaza.
        await Assert.ThrowsAsync<ModelException>(() => service!.PonerPublicoAsync(usuarioId + 9999));

        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Usuario_PonerPrivado_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:Usuario:ponerPrivado:arrange) ENABLED START*/
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var usuarioService = new UsuarioService(scenario.UnitOfWork);
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "UsuarioPrivado");
        Assert.True(usuario.Publico);
        int usuarioId = usuario.Id;
        /*PROTECTED REGION END*/

        var operationName = "ponerPrivado";
        var operationType = "Custom";
        var executeManualInvocation = true;
        UsuarioService? service = usuarioService;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Usuario:ponerPrivado:invoke) ENABLED START*/
        await service!.PonerPrivadoAsync(usuarioId);
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Usuario:ponerPrivado:assert) ENABLED START*/
        await using (var fresh = scenario.CreateFreshUnitOfWork())
        {
            var usuarioBD = await fresh.UsuarioRepository.ReadAsync(usuarioId);
            Assert.NotNull(usuarioBD);
            Assert.False(usuarioBD!.Publico);
        }

        // Ya es privado: una segunda llamada debe rechazarse con ModelException.
        await Assert.ThrowsAsync<ModelException>(() => service!.PonerPrivadoAsync(usuarioId));
        // Un usuario inexistente también se rechaza.
        await Assert.ThrowsAsync<ModelException>(() => service!.PonerPrivadoAsync(usuarioId + 9999));

        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Nevera_RecorrerListaCompra_CustomTransaction_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:customTransaction:Nevera:recorrerListaCompra:arrange) ENABLED START*/
        /*PROTECTED REGION END*/
        var operationName = "recorrerListaCompra";
        var operationType = "CustomTransaction";
        var executeManualInvocation = false;
        NeveraUseCase? useCase = null;
        object? invocationResult = null;
        if (executeManualInvocation) {
        /*PROTECTED REGION ID(test:customTransaction:Nevera:recorrerListaCompra:invoke) ENABLED START*/
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;
        /*PROTECTED REGION ID(test:customTransaction:Nevera:recorrerListaCompra:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Receta_Compartir_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:Receta:compartir:arrange) ENABLED START*/
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var recetaService = new RecetaService(scenario.UnitOfWork);
        var chef = await RecetaScenarioHelper.CrearChefAsync(scenario.UnitOfWork, "ChefCompartir");
        var receta = await RecetaScenarioHelper.CrearRecetaAsync(scenario.UnitOfWork, chef, "Receta compartir", DietaEspecial.ninguna, 4.0f, 10, 1);
        int recetaId = receta.Id;
        /*PROTECTED REGION END*/

        var operationName = "compartir";
        var operationType = "Custom";
        var executeManualInvocation = true;
        RecetaService? service = recetaService;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Receta:compartir:invoke) ENABLED START*/
        await service!.CompartirAsync(recetaId);
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Receta:compartir:assert) ENABLED START*/
        var recetaBD = await scenario.UnitOfWork.RecetaRepository.ReadAsync(recetaId);
        Assert.NotNull(recetaBD);

        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Receta_ComprobarIngredientesDisponibles_CustomTransaction_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:customTransaction:Receta:comprobarIngredientesDisponibles:arrange) ENABLED START*/
        /*PROTECTED REGION END*/
        var operationName = "comprobarIngredientesDisponibles";
        var operationType = "CustomTransaction";
        var executeManualInvocation = false;
        RecetaUseCase? useCase = null;
        object? invocationResult = null;
        if (executeManualInvocation) {
        /*PROTECTED REGION ID(test:customTransaction:Receta:comprobarIngredientesDisponibles:invoke) ENABLED START*/
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;
        /*PROTECTED REGION ID(test:customTransaction:Receta:comprobarIngredientesDisponibles:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task IngredienteCompra_MarcarComprado_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:IngredienteCompra:marcarComprado:arrange) ENABLED START*/
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var ingredienteCompraService = new IngredienteCompraService(scenario.UnitOfWork);
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "UsuarioCompra");
        var ingrediente = await RecetaScenarioHelper.CrearIngredienteAsync(scenario.UnitOfWork, "Leche");
        var item = new IngredienteCompra { Cantidad = 1, Comprado = false, Nevera = usuario.Nevera, ListaCompra = usuario.ListaCompra, IngredienteCatalogo = ingrediente };
        await scenario.UnitOfWork.IngredienteCompraRepository.SaveAsync(item);
        int itemId = item.Id;
        /*PROTECTED REGION END*/

        var operationName = "marcarComprado";
        var operationType = "Custom";
        var executeManualInvocation = true;
        IngredienteCompraService? service = ingredienteCompraService;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:IngredienteCompra:marcarComprado:invoke) ENABLED START*/
        await service!.MarcarCompradoAsync(itemId);
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:IngredienteCompra:marcarComprado:assert) ENABLED START*/
        await using var fresh = scenario.CreateFreshUnitOfWork();
        var itemBD = await fresh.IngredienteCompraRepository.ReadAsync(itemId);
        Assert.NotNull(itemBD);
        Assert.True(itemBD!.Comprado);
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task IngredienteCompra_DesmarcarComprado_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:IngredienteCompra:desmarcarComprado:arrange) ENABLED START*/
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var ingredienteCompraService = new IngredienteCompraService(scenario.UnitOfWork);
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "UsuarioCompra");
        var ingrediente = await RecetaScenarioHelper.CrearIngredienteAsync(scenario.UnitOfWork, "Leche");
        var item = new IngredienteCompra { Cantidad = 1, Comprado = true, Nevera = usuario.Nevera, ListaCompra = usuario.ListaCompra, IngredienteCatalogo = ingrediente };
        await scenario.UnitOfWork.IngredienteCompraRepository.SaveAsync(item);
        int itemId = item.Id;
        /*PROTECTED REGION END*/

        var operationName = "desmarcarComprado";
        var operationType = "Custom";
        var executeManualInvocation = true;
        IngredienteCompraService? service = ingredienteCompraService;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:IngredienteCompra:desmarcarComprado:invoke) ENABLED START*/
        await service!.DesmarcarCompradoAsync(itemId);
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:IngredienteCompra:desmarcarComprado:assert) ENABLED START*/
        await using var fresh = scenario.CreateFreshUnitOfWork();
        var itemBD = await fresh.IngredienteCompraRepository.ReadAsync(itemId);
        Assert.NotNull(itemBD);
        Assert.False(itemBD!.Comprado);
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task IngredienteNevera_ComprobarCaducidad_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:IngredienteNevera:comprobarCaducidad:arrange) ENABLED START*/
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var ingredienteNeveraService = new IngredienteNeveraService(scenario.UnitOfWork);
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "UsuarioCaducidad");
        var ingrediente = await RecetaScenarioHelper.CrearIngredienteAsync(scenario.UnitOfWork, "Yogur");
        var item = new IngredienteNevera { Cantidad = 3, FechaCaducidad = DateTime.UtcNow.Date.AddDays(-2), IngredienteCatalogo = ingrediente, Nevera = usuario.Nevera };
        await scenario.UnitOfWork.IngredienteNeveraRepository.SaveAsync(item);
        int itemId = item.Id;
        /*PROTECTED REGION END*/

        var operationName = "comprobarCaducidad";
        var operationType = "Custom";
        var executeManualInvocation = true;
        IngredienteNeveraService? service = ingredienteNeveraService;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:IngredienteNevera:comprobarCaducidad:invoke) ENABLED START*/
        await service!.ComprobarCaducidadAsync(itemId);
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:IngredienteNevera:comprobarCaducidad:assert) ENABLED START*/
        await using var fresh = scenario.CreateFreshUnitOfWork();
        var itemBD = await fresh.IngredienteNeveraRepository.ReadAsync(itemId);
        Assert.NotNull(itemBD);
        Assert.Equal(0, itemBD!.Cantidad);

        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Comentario_RecalcularValoracionMedia_CustomTransaction_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:customTransaction:Comentario:recalcularValoracionMedia:arrange) ENABLED START*/
        /*PROTECTED REGION END*/
        var operationName = "recalcularValoracionMedia";
        var operationType = "CustomTransaction";
        var executeManualInvocation = false;
        ComentarioUseCase? useCase = null;
        object? invocationResult = null;
        if (executeManualInvocation) {
        /*PROTECTED REGION ID(test:customTransaction:Comentario:recalcularValoracionMedia:invoke) ENABLED START*/
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;
        /*PROTECTED REGION ID(test:customTransaction:Comentario:recalcularValoracionMedia:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    private const string SuccessColor = "\u001b[32m";
    private const string ErrorColor = "\u001b[31m";
    private const string ResetColor = "\u001b[0m";

    private void EnsureContractOutput(string operationName, string operationType)
    {
        Ensure(!string.IsNullOrWhiteSpace(operationName), $"{operationName} declares the manual operation name.");
        Ensure(!string.IsNullOrWhiteSpace(operationType), $"{operationName} declares the manual operation type.");
        WriteTestMessage(true, $"{operationName} contract uses operation type {operationType}.");
    }

    private void Ensure(bool condition, string message)
    {
        WriteTestMessage(condition, message);
        Assert.True(condition, message);
    }

    private void WriteTestMessage(bool condition, string message)
    {
        var color = condition ? SuccessColor : ErrorColor;
        var status = condition ? "[OK] " : "[FAIL] ";
        _output.WriteLine($"{color}{status}{message}{ResetColor}");
    }
}