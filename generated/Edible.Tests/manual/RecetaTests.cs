using Xunit;
using Edible.Application.Exceptions;
using Edible.Application.UseCases.Receta;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using System.Linq;
using System.Threading.Tasks;

namespace Edible.Tests.Manual;

public class RecetaTests
{
    [Fact]
    public async Task ComprobarIngredientesDisponibles_StockSuficiente_DevuelveRecetaYConfirma()
    {
        // Arrange
        await using var scenario = await TestFixture.CreateScenarioAsync();
        
        // Creamos el ciclo de Nevera y Usuario guardando primero el contenedor principal con cascada
        var usuario = new Usuario();
        var nevera = new Nevera { Usuario = usuario };
        usuario.Nevera = nevera;

        await scenario.UnitOfWork.NeveraRepository.SaveAsync(nevera);

        var chef = new Chef { Nevera = nevera };
        await scenario.UnitOfWork.ChefRepository.SaveAsync(chef);

        var ingrediente = new Ingrediente { Nombre = "Huevos" };
        await scenario.UnitOfWork.IngredienteRepository.SaveAsync(ingrediente);

        var ingNevera = new IngredienteNevera { Cantidad = 6, IngredienteCatalogo = ingrediente, Nevera = nevera };
        await scenario.UnitOfWork.IngredienteNeveraRepository.SaveAsync(ingNevera);

        var receta = new Receta { Nombre = "Tortilla", Descripcion = "Rica", DietaEspecial = DietaEspecial.ninguna, ValoracionMedia = 5.0f, Chef = chef };
        await scenario.UnitOfWork.RecetaRepository.SaveAsync(receta);

        var linea = new LineaReceta { CantidadNecesaria = 4, IngredienteCatalogo = ingrediente, Receta = receta };
        await scenario.UnitOfWork.LineaRecetaRepository.SaveAsync(linea);

        var useCase = new RecetaUseCase(scenario.UnitOfWork);

        // Act
        var resultado = await useCase.ComprobarIngredientesDisponiblesAsync("Tortilla", "Desc", DietaEspecial.ninguna, 5.0f, 1);
        
        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Tortilla", resultado.Nombre);
    }

    [Fact]
    public async Task ComprobarIngredientesDisponibles_SinStock_LanzaModelException()
    {
        // Arrange
        await using var scenario = await TestFixture.CreateScenarioAsync();
        
        var usuario = new Usuario();
        var nevera = new Nevera { Usuario = usuario };
        usuario.Nevera = nevera;

        await scenario.UnitOfWork.NeveraRepository.SaveAsync(nevera);

        var chef = new Chef { Nevera = nevera };
        await scenario.UnitOfWork.ChefRepository.SaveAsync(chef);

        var ingrediente = new Ingrediente { Nombre = "Huevos" };
        await scenario.UnitOfWork.IngredienteRepository.SaveAsync(ingrediente);

        var ingNevera = new IngredienteNevera { Cantidad = 2, IngredienteCatalogo = ingrediente, Nevera = nevera };
        await scenario.UnitOfWork.IngredienteNeveraRepository.SaveAsync(ingNevera);

        var receta = new Receta { Nombre = "Tortilla", Descripcion = "Rica", DietaEspecial = DietaEspecial.ninguna, Chef = chef };
        await scenario.UnitOfWork.RecetaRepository.SaveAsync(receta);

        var linea = new LineaReceta { CantidadNecesaria = 4, IngredienteCatalogo = ingrediente, Receta = receta };
        await scenario.UnitOfWork.LineaRecetaRepository.SaveAsync(linea);

        var useCase = new RecetaUseCase(scenario.UnitOfWork);

        // Act & Assert
        await Assert.ThrowsAsync<ModelException>(() => 
            useCase.ComprobarIngredientesDisponiblesAsync("Tortilla", "Desc", DietaEspecial.ninguna, 5.0f, 1));
    }

    [Fact]
    public async Task BuscarNombre_ReadFilter_DevuelveSoloRecetasCoincidentes()
    {
        // Arrange
        await using var scenario = await TestFixture.CreateScenarioAsync();
        
        var usuario = new Usuario();
        var nevera = new Nevera { Usuario = usuario };
        usuario.Nevera = nevera;

        await scenario.UnitOfWork.NeveraRepository.SaveAsync(nevera);

        var chef = new Chef { Nevera = nevera };
        await scenario.UnitOfWork.ChefRepository.SaveAsync(chef);

        var receta1 = new Receta { Nombre = "Tortilla de patatas", Chef = chef };
        var receta2 = new Receta { Nombre = "Sopa de ajo", Chef = chef };
        
        await scenario.UnitOfWork.RecetaRepository.SaveAsync(receta1);
        await scenario.UnitOfWork.RecetaRepository.SaveAsync(receta2);
        
        // Act
        var resultados = await scenario.UnitOfWork.RecetaRepository.BuscarNombreAsync("Tortilla");
        
        // Assert
        Assert.Single(resultados);
        Assert.Contains("Tortilla", resultados.First().Nombre);
    }

    [Fact]
    public async Task RecetasPorNevera_ReadFilter_EjecutaCorrectamente()
    {
        // Arrange
        await using var scenario = await TestFixture.CreateScenarioAsync();
        
        var usuario = new Usuario();
        var nevera = new Nevera { Usuario = usuario };
        usuario.Nevera = nevera;

        await scenario.UnitOfWork.NeveraRepository.SaveAsync(nevera);
        
        // Act
        var resultados = await scenario.UnitOfWork.RecetaRepository.RecetasPorNeveraAsync(nevera.Id);
        
        // Assert
        Assert.NotNull(resultados);
    }
}