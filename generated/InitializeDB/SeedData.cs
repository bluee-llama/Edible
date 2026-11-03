using Edible.Application.Services;
using Edible.Application.UseCases.Comentario;
using Edible.Application.UseCases.Nevera;
using Edible.Application.UseCases.Receta;
using Edible.Domain.Entities;

using Edible.Domain.Enums;
using Edible.Infrastructure;
using Edible.Infrastructure.Repositories;
using Edible.Infrastructure.Security;
using Microsoft.Extensions.Logging;

/*PROTECTED REGION ID(seedDataUsings) ENABLED START*/
// Add custom usings here.
/*PROTECTED REGION END*/

namespace InitializeDB;

public static class SeedData
{
    public static async Task InitializeDataAsync(NHibernate.Cfg.Configuration cfg, ILoggerFactory? loggerFactory = null)
    {
        try
        {
            using var unitOfWorkFactory = new UnitOfWorkFactory(cfg, loggerFactory);

            var chefService = new ChefService(unitOfWorkFactory);
            var sampleChef = await chefService.NewAsync("Sample Chef Name", "Password123!", true, DietaEspecial.ninguna, new Nevera(), new ListaCompra());
            var sampleNevera = sampleChef.Nevera;
            var sampleListaCompra = sampleChef.ListaCompra;
            Ensure(sampleChef.Nevera?.Equals(sampleNevera) == true, "Chef.new relates sampleChef and sampleNevera.");
            Ensure(sampleChef.ListaCompra?.Equals(sampleListaCompra) == true, "Chef.new relates sampleChef and sampleListaCompra.");
            Ensure(sampleNevera.Usuario?.Equals(sampleChef) == true, "Chef.new relates sampleNevera and sampleChef.");
            Ensure(sampleListaCompra.Usuario?.Equals(sampleChef) == true, "Chef.new relates sampleListaCompra and sampleChef.");
            var sampleChefReadBack = await chefService.ReadOIDAsync(sampleChef.Id);
            Ensure(sampleChefReadBack is not null, "Chef read by OID returns the generated entity.");
            var sampleChefReadAll = await chefService.ReadAllAsync();
            Ensure(sampleChefReadAll.Count > 0, "Chef read all returns generated data.");
            await chefService.ModifyAsync(sampleChef.Id, "Sample Chef Name", "Password123!", true, DietaEspecial.ninguna);
            Ensure(await chefService.ReadOIDAsync(sampleChef.Id) is not null, "Chef modify keeps the entity readable.");

            var ingredienteService = new IngredienteService(unitOfWorkFactory);
            var sampleIngrediente = await ingredienteService.NewAsync("Sample Ingrediente Nombre", TipoIngrediente.ninguno, 7, 7.5f, 7.5f, 7.5f, 7.5f, new List<Alergenos> { Alergenos.ninguno }, new List<DietaEspecial> { DietaEspecial.ninguna });
            var sampleIngredienteReadBack = await ingredienteService.ReadOIDAsync(sampleIngrediente.Id);
            Ensure(sampleIngredienteReadBack is not null, "Ingrediente read by OID returns the generated entity.");
            var sampleIngredienteReadAll = await ingredienteService.ReadAllAsync();
            Ensure(sampleIngredienteReadAll.Count > 0, "Ingrediente read all returns generated data.");
            await ingredienteService.ModifyAsync(sampleIngrediente.Id, "Sample Ingrediente Nombre", TipoIngrediente.ninguno, 7, 7.5f, 7.5f, 7.5f, 7.5f, new List<Alergenos> { Alergenos.ninguno }, new List<DietaEspecial> { DietaEspecial.ninguna });
            Ensure(await ingredienteService.ReadOIDAsync(sampleIngrediente.Id) is not null, "Ingrediente modify keeps the entity readable.");

            var recetaService = new RecetaService(unitOfWorkFactory);
            var sampleReceta = await recetaService.NewAsync("Sample Receta Nombre", "Sample Receta Descripcion", DietaEspecial.ninguna, 5.5f, 5, 5, sampleChef.Id, new List<LineaReceta> { new LineaReceta { CantidadNecesaria = 6, IngredienteCatalogo = sampleIngrediente } });
            var sampleLineaReceta = sampleReceta.Lineas.First();
            Ensure(sampleReceta.Chef?.Equals(sampleChef) == true, "Receta.new relates sampleReceta and sampleChef.");
            Ensure(sampleReceta.Lineas.Contains(sampleLineaReceta), "Receta.new relates sampleReceta and sampleLineaReceta.");
            Ensure(sampleLineaReceta.Receta?.Equals(sampleReceta) == true, "Receta.new relates sampleLineaReceta and sampleReceta.");
            Ensure(sampleLineaReceta.IngredienteCatalogo?.Equals(sampleIngrediente) == true, "Receta.new relates sampleLineaReceta and sampleIngrediente.");
            var sampleRecetaReadBack = await recetaService.ReadOIDAsync(sampleReceta.Id);
            Ensure(sampleRecetaReadBack is not null, "Receta read by OID returns the generated entity.");
            var sampleRecetaReadAll = await recetaService.ReadAllAsync();
            Ensure(sampleRecetaReadAll.Count > 0, "Receta read all returns generated data.");
            var sampleRecetaBusquedaAvanzadaResult = await recetaService.BusquedaAvanzadaAsync(DietaEspecial.ninguna, 5.5f, 5, 5, 5, Alergenos.ninguno, sampleIngrediente.Id);
            Ensure(sampleRecetaBusquedaAvanzadaResult is not null, "Receta.busquedaAvanzada filter executes.");
            var sampleRecetaBuscarNombreResult = await recetaService.BuscarNombreAsync("Sample Receta Nombre");
            Ensure(sampleRecetaBuscarNombreResult is not null, "Receta.buscarNombre filter executes.");
            var sampleRecetaRecetasPorNeveraResult = await recetaService.RecetasPorNeveraAsync(sampleNevera.Id);
            Ensure(sampleRecetaRecetasPorNeveraResult is not null, "Receta.recetasPorNevera filter executes.");
            await recetaService.ModifyAsync(sampleReceta.Id, "Sample Receta Nombre", "Sample Receta Descripcion", DietaEspecial.ninguna, 5.5f, 5, 5, sampleChef.Id);
            Ensure(await recetaService.ReadOIDAsync(sampleReceta.Id) is not null, "Receta modify keeps the entity readable.");

            var pasoService = new PasoService(unitOfWorkFactory);
            var samplePaso = await pasoService.NewAsync("Sample Paso Nombre", "Sample Paso Descripcion", sampleReceta.Id);
            Ensure(samplePaso.Receta?.Equals(sampleReceta) == true, "Paso.new relates samplePaso and sampleReceta.");
            var samplePasoReadBack = await pasoService.ReadOIDAsync(samplePaso.Id);
            Ensure(samplePasoReadBack is not null, "Paso read by OID returns the generated entity.");
            var samplePasoReadAll = await pasoService.ReadAllAsync();
            Ensure(samplePasoReadAll.Count > 0, "Paso read all returns generated data.");
            await pasoService.ModifyAsync(samplePaso.Id, "Sample Paso Nombre", "Sample Paso Descripcion");
            Ensure(await pasoService.ReadOIDAsync(samplePaso.Id) is not null, "Paso modify keeps the entity readable.");

            var usuarioService = new UsuarioService(unitOfWorkFactory);
            var sampleUsuario = await usuarioService.NewAsync("Sample Usuario Name", "Password123!", true, DietaEspecial.ninguna, sampleNevera, sampleListaCompra);
            var sampleUsuarioLoginToken = await usuarioService.LoginAsync(sampleUsuario.Id, "Password123!");
            Ensure(!string.IsNullOrWhiteSpace(sampleUsuarioLoginToken), "Usuario.login returns a login token for sampleUsuario.");
            Ensure(sampleUsuario.Nevera?.Equals(sampleNevera) == true, "Usuario.new relates sampleUsuario and sampleNevera.");
            Ensure(sampleUsuario.ListaCompra?.Equals(sampleListaCompra) == true, "Usuario.new relates sampleUsuario and sampleListaCompra.");
            var sampleUsuarioReadBack = await usuarioService.ReadOIDAsync(sampleUsuario.Id);
            Ensure(sampleUsuarioReadBack is not null, "Usuario read by OID returns the generated entity.");
            var sampleUsuarioReadAll = await usuarioService.ReadAllAsync();
            Ensure(sampleUsuarioReadAll.Count > 0, "Usuario read all returns generated data.");
            await usuarioService.ModifyAsync(sampleUsuario.Id, "Sample Usuario Name", "Password123!", true, DietaEspecial.ninguna, default!, default!);
            Ensure(await usuarioService.ReadOIDAsync(sampleUsuario.Id) is not null, "Usuario modify keeps the entity readable.");

            var ingredienteCompraService = new IngredienteCompraService(unitOfWorkFactory);
            var sampleIngredienteCompra = await ingredienteCompraService.NewAsync(8, new DateTime(2026, 1, 8), true, sampleNevera.Id, sampleListaCompra.Id, sampleIngrediente.Id);
            Ensure(sampleIngredienteCompra.Nevera?.Equals(sampleNevera) == true, "IngredienteCompra.new relates sampleIngredienteCompra and sampleNevera.");
            Ensure(sampleIngredienteCompra.ListaCompra?.Equals(sampleListaCompra) == true, "IngredienteCompra.new relates sampleIngredienteCompra and sampleListaCompra.");
            Ensure(sampleIngredienteCompra.IngredienteCatalogo?.Equals(sampleIngrediente) == true, "IngredienteCompra.new relates sampleIngredienteCompra and sampleIngrediente.");
            var sampleIngredienteCompraReadBack = await ingredienteCompraService.ReadOIDAsync(sampleIngredienteCompra.Id);
            Ensure(sampleIngredienteCompraReadBack is not null, "IngredienteCompra read by OID returns the generated entity.");
            var sampleIngredienteCompraReadAll = await ingredienteCompraService.ReadAllAsync();
            Ensure(sampleIngredienteCompraReadAll.Count > 0, "IngredienteCompra read all returns generated data.");
            await ingredienteCompraService.ModifyAsync(sampleIngredienteCompra.Id, 8, new DateTime(2026, 1, 8), true, sampleNevera.Id, sampleListaCompra.Id, sampleIngrediente.Id);
            Ensure(await ingredienteCompraService.ReadOIDAsync(sampleIngredienteCompra.Id) is not null, "IngredienteCompra modify keeps the entity readable.");

            var ingredienteNeveraService = new IngredienteNeveraService(unitOfWorkFactory);
            var sampleIngredienteNevera = await ingredienteNeveraService.NewAsync(9, new DateTime(2026, 1, 9), sampleIngrediente.Id, sampleNevera.Id, sampleReceta.Id);
            Ensure(sampleIngredienteNevera.IngredienteCatalogo?.Equals(sampleIngrediente) == true, "IngredienteNevera.new relates sampleIngredienteNevera and sampleIngrediente.");
            Ensure(sampleIngredienteNevera.Nevera?.Equals(sampleNevera) == true, "IngredienteNevera.new relates sampleIngredienteNevera and sampleNevera.");
            Ensure(sampleIngredienteNevera.Receta?.Equals(sampleReceta) == true, "IngredienteNevera.new relates sampleIngredienteNevera and sampleReceta.");
            var sampleIngredienteNeveraReadBack = await ingredienteNeveraService.ReadOIDAsync(sampleIngredienteNevera.Id);
            Ensure(sampleIngredienteNeveraReadBack is not null, "IngredienteNevera read by OID returns the generated entity.");
            var sampleIngredienteNeveraReadAll = await ingredienteNeveraService.ReadAllAsync();
            Ensure(sampleIngredienteNeveraReadAll.Count > 0, "IngredienteNevera read all returns generated data.");
            var sampleIngredienteNeveraFiltrarNeveraResult = await ingredienteNeveraService.FiltrarNeveraAsync(sampleNevera.Id, TipoIngrediente.ninguno, new DateTime(2026, 1, 9), new DateTime(2026, 1, 9), EstadoCaducidad.ninguno);
            Ensure(sampleIngredienteNeveraFiltrarNeveraResult is not null, "IngredienteNevera.filtrarNevera filter executes.");
            await ingredienteNeveraService.ModifyAsync(sampleIngredienteNevera.Id, 9, new DateTime(2026, 1, 9));
            Ensure(await ingredienteNeveraService.ReadOIDAsync(sampleIngredienteNevera.Id) is not null, "IngredienteNevera modify keeps the entity readable.");

            var comentarioService = new ComentarioService(unitOfWorkFactory);
            var sampleComentario = await comentarioService.NewAsync(10, "Sample Comentario Titulo", 10, sampleUsuario.Id, sampleReceta.Id);
            Ensure(sampleComentario.Comentador?.Equals(sampleUsuario) == true, "Comentario.new relates sampleComentario and sampleUsuario.");
            Ensure(sampleComentario.Receta?.Equals(sampleReceta) == true, "Comentario.new relates sampleComentario and sampleReceta.");
            var sampleComentarioReadBack = await comentarioService.ReadOIDAsync(sampleComentario.Id);
            Ensure(sampleComentarioReadBack is not null, "Comentario read by OID returns the generated entity.");
            var sampleComentarioReadAll = await comentarioService.ReadAllAsync();
            Ensure(sampleComentarioReadAll.Count > 0, "Comentario read all returns generated data.");
            await comentarioService.ModifyAsync(sampleComentario.Id, 10, "Sample Comentario Titulo", 10);
            Ensure(await comentarioService.ReadOIDAsync(sampleComentario.Id) is not null, "Comentario modify keeps the entity readable.");

            await pasoService.DestroyAsync(samplePaso.Id);
            await ingredienteCompraService.DestroyAsync(sampleIngredienteCompra.Id);
            await ingredienteNeveraService.DestroyAsync(sampleIngredienteNevera.Id);
            await comentarioService.DestroyAsync(sampleComentario.Id);
            await recetaService.DestroyAsync(sampleReceta.Id);
            await chefService.DestroyAsync(sampleChef.Id);
            await usuarioService.DestroyAsync(sampleUsuario.Id);
            await ingredienteService.DestroyAsync(sampleIngrediente.Id);

            Ensure((await chefService.ReadAllAsync()).Count == 0, "Chef seed cleanup completed.");
            Ensure((await ingredienteService.ReadAllAsync()).Count == 0, "Ingrediente seed cleanup completed.");
            Ensure((await recetaService.ReadAllAsync()).Count == 0, "Receta seed cleanup completed.");
            Ensure((await pasoService.ReadAllAsync()).Count == 0, "Paso seed cleanup completed.");
            Ensure((await usuarioService.ReadAllAsync()).Count == 0, "Usuario seed cleanup completed.");
            Ensure((await ingredienteCompraService.ReadAllAsync()).Count == 0, "IngredienteCompra seed cleanup completed.");
            Ensure((await ingredienteNeveraService.ReadAllAsync()).Count == 0, "IngredienteNevera seed cleanup completed.");
            Ensure((await comentarioService.ReadAllAsync()).Count == 0, "Comentario seed cleanup completed.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException ?? ex);
            throw;
        }
    }

    public static async Task InitializeDataManualAsync(NHibernate.Cfg.Configuration cfg, ILoggerFactory? loggerFactory = null)
    {
        try
        {
            using var unitOfWorkFactory = new UnitOfWorkFactory(cfg, loggerFactory);

            /*PROTECTED REGION ID(initializeDataManual) ENABLED START*/
            /*PROTECTED REGION END*/
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.InnerException ?? ex);
            throw;
        }
    }

    private static void Ensure(bool condition, string message)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = condition ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine((condition ? "[OK] " : "[FAIL] ") + message);
        Console.ForegroundColor = previousColor;

        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }
}