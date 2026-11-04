using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Infrastructure;

namespace Edible.Tests.Manual;

public class BusquedaAvanzadaTests
{
    // Datos:
    //  Pizza    vegetariana  4.5  40min  dif 2  [harina(gluten), tomate, queso(lactosa)]
    //  Ensalada vegana       3.0  10min  dif 1  [tomate]
    //  Pasta    vegana       4.0  25min  dif 2  [harina(gluten), tomate]
    private sealed class Datos
    {
        public int QuesoId { get; init; }
        public int TomateId { get; init; }
    }

    private static async Task<Datos> PrepararAsync(UnitOfWork uow)
    {
        var chef = await RecetaScenarioHelper.CrearChefAsync(uow, "chef");
        var harina = await RecetaScenarioHelper.CrearIngredienteAsync(uow, "Harina", Alergenos.gluten);
        var tomate = await RecetaScenarioHelper.CrearIngredienteAsync(uow, "Tomate");
        var queso = await RecetaScenarioHelper.CrearIngredienteAsync(uow, "Queso", Alergenos.lactosa);

        await RecetaScenarioHelper.CrearRecetaAsync(uow, chef, "Pizza", DietaEspecial.vegetariana, 4.5f, 40, 2, harina, tomate, queso);
        await RecetaScenarioHelper.CrearRecetaAsync(uow, chef, "Ensalada", DietaEspecial.vegana, 3.0f, 10, 1, tomate);
        await RecetaScenarioHelper.CrearRecetaAsync(uow, chef, "Pasta", DietaEspecial.vegana, 4.0f, 25, 2, harina, tomate);
        return new Datos { QuesoId = queso.Id, TomateId = tomate.Id };
    }

    private static async Task<string[]> BuscarAsync(
        UnitOfWork uow,
        DietaEspecial dieta = DietaEspecial.ninguna, float nota = 0f,
        int tiempoMin = 0, int tiempoMax = 0, int dificultad = 0,
        Alergenos alergeno = Alergenos.ninguno, int ingredienteOid = 0)
    {
        var res = await uow.RecetaRepository.BusquedaAvanzadaAsync(dieta, nota, tiempoMin, tiempoMax, dificultad, alergeno, ingredienteOid);
        Assert.NotNull(res);
        return res.Select(r => r.Nombre).OrderBy(n => n).ToArray();
    }

    [Fact]
    public async Task BusquedaAvanzada_SinFiltros_DevuelveTodas()
    {
        await using var s = await TestFixture.CreateScenarioAsync();
        await PrepararAsync(s.UnitOfWork);
        Assert.Equal(new[] { "Ensalada", "Pasta", "Pizza" }, await BuscarAsync(s.UnitOfWork));
    }

    [Fact]
    public async Task BusquedaAvanzada_PorDieta_DevuelveSoloEsaDieta()
    {
        await using var s = await TestFixture.CreateScenarioAsync();
        await PrepararAsync(s.UnitOfWork);
        Assert.Equal(new[] { "Ensalada", "Pasta" }, await BuscarAsync(s.UnitOfWork, dieta: DietaEspecial.vegana));
    }

    [Fact]
    public async Task BusquedaAvanzada_NotaMinima_IncluyeLaFrontera()
    {
        await using var s = await TestFixture.CreateScenarioAsync();
        await PrepararAsync(s.UnitOfWork);
        Assert.Equal(new[] { "Pasta", "Pizza" }, await BuscarAsync(s.UnitOfWork, nota: 4.0f));
    }

    [Fact]
    public async Task BusquedaAvanzada_RangoDeTiempo_IncluyeLosLimites()
    {
        await using var s = await TestFixture.CreateScenarioAsync();
        await PrepararAsync(s.UnitOfWork);
        Assert.Equal(new[] { "Pasta", "Pizza" }, await BuscarAsync(s.UnitOfWork, tiempoMin: 25, tiempoMax: 40));
    }

    [Fact]
    public async Task BusquedaAvanzada_PorDificultad_DevuelveSoloEsaDificultad()
    {
        await using var s = await TestFixture.CreateScenarioAsync();
        await PrepararAsync(s.UnitOfWork);
        Assert.Equal(new[] { "Ensalada" }, await BuscarAsync(s.UnitOfWork, dificultad: 1));
    }

    [Fact]
    public async Task BusquedaAvanzada_ExcluirGluten_NoDevuelveRecetasConHarina()
    {
        await using var s = await TestFixture.CreateScenarioAsync();
        await PrepararAsync(s.UnitOfWork);
        Assert.Equal(new[] { "Ensalada" }, await BuscarAsync(s.UnitOfWork, alergeno: Alergenos.gluten));
    }

    [Fact]
    public async Task BusquedaAvanzada_ExcluirLactosa_NoDevuelveRecetasConQueso()
    {
        await using var s = await TestFixture.CreateScenarioAsync();
        await PrepararAsync(s.UnitOfWork);
        Assert.Equal(new[] { "Ensalada", "Pasta" }, await BuscarAsync(s.UnitOfWork, alergeno: Alergenos.lactosa));
    }

    [Fact]
    public async Task BusquedaAvanzada_PorIngrediente_DevuelveSoloLasQueLoContienen()
    {
        await using var s = await TestFixture.CreateScenarioAsync();
        var d = await PrepararAsync(s.UnitOfWork);
        Assert.Equal(new[] { "Pizza" }, await BuscarAsync(s.UnitOfWork, ingredienteOid: d.QuesoId));
    }

    [Fact]
    public async Task BusquedaAvanzada_FiltrosCombinados_DevuelveSoloLaQueCumpleTodos()
    {
        await using var s = await TestFixture.CreateScenarioAsync();
        var d = await PrepararAsync(s.UnitOfWork);
        var res = await BuscarAsync(s.UnitOfWork,
            dieta: DietaEspecial.vegana, nota: 3.5f, alergeno: Alergenos.lactosa, ingredienteOid: d.TomateId);
        Assert.Equal(new[] { "Pasta" }, res);
    }

    [Fact]
    public async Task BusquedaAvanzada_SinCoincidencias_DevuelveListaVaciaNoNull()
    {
        await using var s = await TestFixture.CreateScenarioAsync();
        await PrepararAsync(s.UnitOfWork);
        Assert.Empty(await BuscarAsync(s.UnitOfWork, dificultad: 3));
    }
}