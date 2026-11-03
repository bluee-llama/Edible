using Edible.Application.Services;
using Edible.Application.UseCases.Comentario;
using Edible.Application.UseCases.Nevera;
using Edible.Application.UseCases.Receta;
using Edible.Domain.Entities;
using Xunit.Abstractions;
using Edible.Domain.Enums;

namespace Edible.Tests.Manual;

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
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "ponerPublico";
        var operationType = "Custom";
        var executeManualInvocation = false;
        UsuarioService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Usuario:ponerPublico:invoke) ENABLED START*/
        await ((dynamic)service!).PonerPublicoAsync(default(int));
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Usuario:ponerPublico:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Usuario_PonerPrivado_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:Usuario:ponerPrivado:arrange) ENABLED START*/
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "ponerPrivado";
        var operationType = "Custom";
        var executeManualInvocation = false;
        UsuarioService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Usuario:ponerPrivado:invoke) ENABLED START*/
        await ((dynamic)service!).PonerPrivadoAsync(default(int));
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Usuario:ponerPrivado:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Nevera_RecorrerListaCompra_CustomTransaction_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:customTransaction:Nevera:recorrerListaCompra:arrange) ENABLED START*/
        // Create a test unit of work and assign useCase before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "recorrerListaCompra";
        var operationType = "CustomTransaction";
        var executeManualInvocation = false;
        NeveraUseCase? useCase = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:customTransaction:Nevera:recorrerListaCompra:invoke) ENABLED START*/
        invocationResult = await ((dynamic)useCase!).RecorrerListaCompraAsync(default(int));
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
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "compartir";
        var operationType = "Custom";
        var executeManualInvocation = false;
        RecetaService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:Receta:compartir:invoke) ENABLED START*/
        await ((dynamic)service!).CompartirAsync(default(int));
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:Receta:compartir:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Receta_ComprobarIngredientesDisponibles_CustomTransaction_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:customTransaction:Receta:comprobarIngredientesDisponibles:arrange) ENABLED START*/
        // Create a test unit of work and assign useCase before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "comprobarIngredientesDisponibles";
        var operationType = "CustomTransaction";
        var executeManualInvocation = false;
        RecetaUseCase? useCase = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:customTransaction:Receta:comprobarIngredientesDisponibles:invoke) ENABLED START*/
        invocationResult = await ((dynamic)useCase!).ComprobarIngredientesDisponiblesAsync("Sample Receta Nombre", "Sample Receta Descripcion", DietaEspecial.vegetariana, 1.5f, default(int));
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
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "marcarComprado";
        var operationType = "Custom";
        var executeManualInvocation = false;
        IngredienteCompraService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:IngredienteCompra:marcarComprado:invoke) ENABLED START*/
        await ((dynamic)service!).MarcarCompradoAsync(default(int));
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:IngredienteCompra:marcarComprado:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task IngredienteCompra_DesmarcarComprado_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:IngredienteCompra:desmarcarComprado:arrange) ENABLED START*/
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "desmarcarComprado";
        var operationType = "Custom";
        var executeManualInvocation = false;
        IngredienteCompraService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:IngredienteCompra:desmarcarComprado:invoke) ENABLED START*/
        await ((dynamic)service!).DesmarcarCompradoAsync(default(int));
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:IngredienteCompra:desmarcarComprado:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task IngredienteNevera_ComprobarCaducidad_Custom_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:custom:IngredienteNevera:comprobarCaducidad:arrange) ENABLED START*/
        // Create a test unit of work and assign service before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "comprobarCaducidad";
        var operationType = "Custom";
        var executeManualInvocation = false;
        IngredienteNeveraService? service = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:custom:IngredienteNevera:comprobarCaducidad:invoke) ENABLED START*/
        await ((dynamic)service!).ComprobarCaducidadAsync(default(int));
        /*PROTECTED REGION END*/
        }
        _ = invocationResult;

        /*PROTECTED REGION ID(test:custom:IngredienteNevera:comprobarCaducidad:assert) ENABLED START*/
        EnsureContractOutput(operationName, operationType);
        /*PROTECTED REGION END*/
    }

    [Fact]
    public async Task Comentario_RecalcularValoracionMedia_CustomTransaction_Manual_Contract()
    {
        /*PROTECTED REGION ID(test:customTransaction:Comentario:recalcularValoracionMedia:arrange) ENABLED START*/
        // Create a test unit of work and assign useCase before enabling the generated invocation.
        /*PROTECTED REGION END*/

        var operationName = "recalcularValoracionMedia";
        var operationType = "CustomTransaction";
        var executeManualInvocation = false;
        ComentarioUseCase? useCase = null;
        object? invocationResult = null;

        if (executeManualInvocation)
        {
        /*PROTECTED REGION ID(test:customTransaction:Comentario:recalcularValoracionMedia:invoke) ENABLED START*/
        invocationResult = await ((dynamic)useCase!).RecalcularValoracionMediaAsync(1, "Sample Comentario Titulo", 1, default(int), default(int));
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
