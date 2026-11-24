using System.Threading.Tasks;
using Edible.Application.Services;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Xunit;

namespace Edible.Tests.Manual;

/// <summary>
/// Respaldan la capa de seguridad de la web (Edible.Web, HariaWebAuthentication.AddTrustedClaims):
/// el rol "Chef" se concede solo si la entidad leída del servidor es una instancia de Chef,
/// así que el repositorio de Usuario DEBE devolver la clase derivada.
/// </summary>
public sealed class SeguridadHerenciaTests
{
    [Fact]
    public async Task UsuarioRepository_Read_DevuelveInstanciaChef_SoloParaUnChef()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var chefService = new ChefService(scenario.UnitOfWork);
        var chef = await chefService.NewAsync("chefSeguridad", "pwd", true, DietaEspecial.ninguna, new Nevera(), new ListaCompra());
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "usuarioSeguridad");
        int chefId = chef.Id;
        int usuarioId = usuario.Id;

        // Sesión nueva: sin entidades en caché del contexto anterior.
        await using var fresh = scenario.CreateFreshUnitOfWork();

        var comoChef = await fresh.UsuarioRepository.ReadAsync(chefId);
        Assert.NotNull(comoChef);
        Assert.IsType<Chef>(comoChef);

        var comoUsuario = await fresh.UsuarioRepository.ReadAsync(usuarioId);
        Assert.NotNull(comoUsuario);
        Assert.False(comoUsuario is Chef);
    }

    [Fact]
    public async Task UsuarioNew_CreaSuNeveraYSuListaCompra_Persistidas()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var usuarioService = new UsuarioService(scenario.UnitOfWork);

        // Es lo mismo que hace Usuario/Register y Usuario/Create en la web: sin elegir nevera ni lista.
        var usuario = await usuarioService.NewAsync("conNevera", "pwd", true, DietaEspecial.ninguna, new Nevera(), new ListaCompra());
        int usuarioId = usuario.Id;

        await using var fresh = scenario.CreateFreshUnitOfWork();
        var bd = await fresh.UsuarioRepository.ReadAsync(usuarioId);

        Assert.NotNull(bd);
        Assert.NotNull(bd!.Nevera);
        Assert.NotNull(bd.ListaCompra);
        Assert.NotEqual(0, bd.Nevera.Id);
        Assert.NotEqual(0, bd.ListaCompra.Id);
    }

    [Fact]
    public async Task ChefNew_CreaSuPropiaNeveraYListaCompra_Persistidas()
    {
        await using var scenario = await TestFixture.CreateScenarioAsync();
        var chefService = new ChefService(scenario.UnitOfWork);
        var usuario = await RecetaScenarioHelper.CrearUsuarioAsync(scenario.UnitOfWork, "otro");

        var chef = await chefService.NewAsync("chefConNevera", "pwd", true, DietaEspecial.ninguna, new Nevera(), new ListaCompra());
        int chefId = chef.Id;

        await using var fresh = scenario.CreateFreshUnitOfWork();
        var bd = await fresh.ChefRepository.ReadAsync(chefId);

        Assert.NotNull(bd);
        Assert.NotNull(bd!.Nevera);
        Assert.NotNull(bd.ListaCompra);
        Assert.NotEqual(usuario.Nevera.Id, bd.Nevera.Id);
        Assert.NotEqual(usuario.ListaCompra.Id, bd.ListaCompra.Id);
    }
}
