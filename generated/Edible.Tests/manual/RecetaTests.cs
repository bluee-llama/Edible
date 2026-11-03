using Xunit;
using Edible.Application.Exceptions;
using System.Linq;
using System.Threading.Tasks;

namespace Edible.Tests.manual;

public class RecetaTests
{
    // 1. Tests para tu CustomTransaction (Éxito y Fallo/Rollback)
    [Fact]
    public async Task ComprobarIngredientesDisponibles_StockSuficiente_DevuelveTrue()
    {
        // Arrange (Pendiente del CreateScenarioAsync de la Persona 1)
        await using var scenario = await CreateScenarioAsync(); 
        
        // Act
        var resultado = await scenario.UseCase.ComprobarIngredientesDisponiblesAsync(
            scenario.RecetaPrueba.Id, 
            scenario.NeveraPrueba.Id);
        
        // Assert
        Assert.True(resultado);
    }

    [Fact]
    public async Task ComprobarIngredientesDisponibles_SinStock_LanzaModelExceptionORetornaFalse()
    {
        // Arrange
        await using var scenario = await CreateScenarioAsync();
        // (Aquí vaciarías la nevera del escenario de prueba)
        
        // Act & Assert (Dependiendo de si tu UseCase lanza excepción o devuelve false)
        await Assert.ThrowsAsync<ModelException>(() => 
            scenario.UseCase.ComprobarIngredientesDisponiblesAsync(
                scenario.RecetaPrueba.Id, 
                scenario.NeveraPrueba.Id));
    }

    // 2. Tests para tus Filtros LINQ (Búsqueda por nombre y recetas por nevera)
    [Fact]
    public async Task BuscarNombre_ReadFilter_DevuelveSoloRecetasCoincidentes()
    {
        // Arrange
        await using var scenario = await CreateScenarioAsync();
        
        // Act
        var resultados = await scenario.UnitOfWork.RecetaRepository.BuscarNombreAsync("Tortilla");
        
        // Assert
        Assert.NotEmpty(resultados);
        Assert.All(resultados, r => Assert.Contains("Tortilla", r.Nombre));
    }

    [Fact]
    public async Task RecetasPorNevera_ReadFilter_DevuelveRecetasCocinables()
    {
        // Arrange
        await using var scenario = await CreateScenarioAsync();
        
        // Act
        var resultados = await scenario.UnitOfWork.RecetaRepository.RecetasPorNeveraAsync(scenario.NeveraPrueba.Id);
        
        // Assert
        Assert.NotNull(resultados);
        // Verificar que la receta devuelta realmente se puede cocinar con los ingredientes del fixture
    }
}