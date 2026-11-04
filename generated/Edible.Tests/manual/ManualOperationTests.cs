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
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate.Dialect;
using NHibernate.Driver;
using Edible.Application.Exceptions;
using Edible.Infrastructure; 
/*PROTECTED REGION END*/

namespace Edible.Tests.Manual;

/*PROTECTED REGION ID(ManualOperationTests_Fixture) ENABLED START*/
public class Scenario : IAsyncDisposable
{
    public ISessionFactory SessionFactory { get; }
    public ISession Session { get; }
    public UnitOfWork UnitOfWork { get; }

    public Scenario(ISessionFactory sessionFactory, ISession session, UnitOfWork unitOfWork)
    {
        SessionFactory = sessionFactory;
        Session = session;
        UnitOfWork = unitOfWork;
    }

    public async ValueTask DisposeAsync()
    {
        Session.Dispose();
        SessionFactory.Dispose();
        await Task.CompletedTask;
    }
}

public static class TestFixture
{
    public static async Task<Scenario> CreateScenarioAsync()
    {
        var configuration = new Configuration();
        configuration.DataBaseIntegration(db => {
            db.Dialect<SQLiteDialect>();
            db.Driver<SQLite20Driver>(); // Driver genérico compatible de NHibernate
            db.ConnectionProvider<NHibernate.Connection.DriverConnectionProvider>();
            db.ConnectionString = "Data Source=:memory:;Mode=Memory;Cache=Shared";
            db.ConnectionReleaseMode = ConnectionReleaseMode.OnClose;
        });

        configuration.AddAssembly(typeof(Edible.Infrastructure.Repositories.RepositoryBase<>).Assembly);

        var sessionFactory = configuration.BuildSessionFactory();
        var session = sessionFactory.OpenSession();

        new SchemaExport(configuration).Execute(false, true, false, session.Connection, null);

        var unitOfWork = new UnitOfWork(session);

        return new Scenario(sessionFactory, session, unitOfWork);
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
        var usuario = new Usuario(); // Sin propiedades custom para evitar errores de atributos
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
        var usuarioBD = await scenario.UnitOfWork.UsuarioRepository.ReadAsync(usuarioId);
        Assert.NotNull(usuarioBD);

        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Usuario_PonerPrivado_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:Usuario:ponerPrivado:arrange) ENABLED START*/
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var usuarioService = new UsuarioService(scenario.UnitOfWork);
        var usuario = new Usuario();
        await scenario.UnitOfWork.UsuarioRepository.SaveAsync(usuario);
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
        var usuarioBD = await scenario.UnitOfWork.UsuarioRepository.ReadAsync(usuarioId);
        Assert.NotNull(usuarioBD);

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
        var receta = new Receta();
        await scenario.UnitOfWork.RecetaRepository.SaveAsync(receta);
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
        var item = new IngredienteCompra();
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
        var itemBD = await scenario.UnitOfWork.IngredienteCompraRepository.ReadAsync(itemId);
        Assert.NotNull(itemBD);
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task IngredienteCompra_DesmarcarComprado_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:IngredienteCompra:desmarcarComprado:arrange) ENABLED START*/
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var ingredienteCompraService = new IngredienteCompraService(scenario.UnitOfWork);
        var item = new IngredienteCompra();
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
        var itemBD = await scenario.UnitOfWork.IngredienteCompraRepository.ReadAsync(itemId);
        Assert.NotNull(itemBD);
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task IngredienteNevera_ComprobarCaducidad_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:IngredienteNevera:comprobarCaducidad:arrange) ENABLED START*/
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var ingredienteNeveraService = new IngredienteNeveraService(scenario.UnitOfWork);
        var item = new IngredienteNevera();
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
        var itemBD = await scenario.UnitOfWork.IngredienteNeveraRepository.ReadAsync(itemId);
        Assert.NotNull(itemBD);

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