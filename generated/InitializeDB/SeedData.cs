using Microsoft.EntityFrameworkCore;
using Edible.Application.Services;
using Edible.Application.UseCases.Comentario;
using Edible.Application.UseCases.Nevera;
using Edible.Application.UseCases.Receta;
using Edible.Infrastructure.Data;
using Edible.Domain.Entities;

using Edible.Domain.Enums;
using Edible.Infrastructure;

using Microsoft.Extensions.Logging;

/*PROTECTED REGION ID(seedDataUsings) ENABLED START*/
// Add custom usings here.
/*PROTECTED REGION END*/

namespace InitializeDB;

public static class SeedData
{
    public static async Task InitializeDataAsync(DbContextOptions<EdibleDbContext> options, ILoggerFactory? loggerFactory = null)
    {
        try
        {
            _ = options;
            _ = loggerFactory;

            await using var db = new EdibleDbContext(options);
            await using var unitOfWork = new UnitOfWork(db, loggerFactory);

            var ingredienteService = new IngredienteService(unitOfWork);
            var sampleIngrediente = await ingredienteService.NewAsync("Sample Ingrediente Nombre", TipoIngrediente.ninguno, 7, 7.5f, 7.5f, 7.5f, 7.5f, new List<Alergenos> { Alergenos.ninguno }, new List<DietaEspecial> { DietaEspecial.ninguna });
            var sampleIngredienteReadBack = await ingredienteService.ReadOIDAsync(sampleIngrediente.Id);
            Ensure(sampleIngredienteReadBack is not null, "Ingrediente read by OID returns the generated entity.");
            var sampleIngredienteReadAll = await ingredienteService.ReadAllAsync();
            Ensure(sampleIngredienteReadAll.Count > 0, "Ingrediente read all returns generated data.");
            await ingredienteService.ModifyAsync(sampleIngrediente.Id, "Sample Ingrediente Nombre", TipoIngrediente.ninguno, 7, 7.5f, 7.5f, 7.5f, 7.5f, new List<Alergenos> { Alergenos.ninguno }, new List<DietaEspecial> { DietaEspecial.ninguna });
            Ensure(await ingredienteService.ReadOIDIngredienteDefaultAsync(sampleIngrediente.Id) is not null, "Ingrediente modify keeps the entity readable.");

            var usuarioService = new UsuarioService(unitOfWork);
            var sampleUsuario = await usuarioService.NewAsync("Sample Usuario Name", "Password123!", true, DietaEspecial.ninguna, new Nevera(), new ListaCompra());
            var sampleUsuarioLoginToken = await usuarioService.LoginAsync(sampleUsuario.Id, "Password123!");
            Ensure(!string.IsNullOrWhiteSpace(sampleUsuarioLoginToken), "Usuario.login returns a login token for sampleUsuario.");
            var sampleNevera = sampleUsuario.Nevera;
            var sampleListaCompra = sampleUsuario.ListaCompra;
            Ensure(sampleUsuario.Nevera?.Equals(sampleNevera) == true, "Usuario.new relates sampleUsuario and sampleNevera.");
            Ensure(sampleUsuario.ListaCompra?.Equals(sampleListaCompra) == true, "Usuario.new relates sampleUsuario and sampleListaCompra.");
            Ensure(sampleNevera.Usuario?.Equals(sampleUsuario) == true, "Usuario.new relates sampleNevera and sampleUsuario.");
            Ensure(sampleListaCompra.Usuario?.Equals(sampleUsuario) == true, "Usuario.new relates sampleListaCompra and sampleUsuario.");
            var sampleUsuarioReadBack = await usuarioService.ReadOIDAsync(sampleUsuario.Id);
            Ensure(sampleUsuarioReadBack is not null, "Usuario read by OID returns the generated entity.");
            var sampleUsuarioReadAll = await usuarioService.ReadAllAsync();
            Ensure(sampleUsuarioReadAll.Count > 0, "Usuario read all returns generated data.");
            await usuarioService.ModifyAsync(sampleUsuario.Id, "Sample Usuario Name", "Password123!", true, DietaEspecial.ninguna, default!, default!);
            Ensure(await usuarioService.ReadOIDUsuarioDefaultAsync(sampleUsuario.Id) is not null, "Usuario modify keeps the entity readable.");

            var chefService = new ChefService(unitOfWork);
            var sampleChef = await chefService.NewAsync("Sample Chef Name", "Password123!", true, DietaEspecial.ninguna, new Nevera(), new ListaCompra());
            Ensure(!sampleChef.Nevera.Equals(sampleNevera), "Chef.new creates its own Nevera.");
            Ensure(!sampleChef.ListaCompra.Equals(sampleListaCompra), "Chef.new creates its own ListaCompra.");
            Ensure(sampleChef.Nevera.Usuario?.Equals(sampleChef) == true, "Chef.new relates sampleChef.Nevera and sampleChef.");
            Ensure(sampleChef.ListaCompra.Usuario?.Equals(sampleChef) == true, "Chef.new relates sampleChef.ListaCompra and sampleChef.");
            var sampleChefReadBack = await chefService.ReadOIDAsync(sampleChef.Id);
            Ensure(sampleChefReadBack is not null, "Chef read by OID returns the generated entity.");
            var sampleChefReadAll = await chefService.ReadAllAsync();
            Ensure(sampleChefReadAll.Count > 0, "Chef read all returns generated data.");
            await chefService.ModifyAsync(sampleChef.Id, "Sample Chef Name", "Password123!", true, DietaEspecial.ninguna);
            Ensure(await chefService.ReadOIDChefDefaultAsync(sampleChef.Id) is not null, "Chef modify keeps the entity readable.");

            var recetaService = new RecetaService(unitOfWork);
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
            Ensure(await recetaService.ReadOIDRecetaDefaultAsync(sampleReceta.Id) is not null, "Receta modify keeps the entity readable.");

            var ingredienteCompraService = new IngredienteCompraService(unitOfWork);
            var sampleIngredienteCompra = await ingredienteCompraService.NewAsync(8, new DateTime(2026, 1, 8), true, sampleNevera.Id, sampleListaCompra.Id, sampleIngrediente.Id);
            Ensure(sampleIngredienteCompra.Nevera?.Equals(sampleNevera) == true, "IngredienteCompra.new relates sampleIngredienteCompra and sampleNevera.");
            Ensure(sampleIngredienteCompra.ListaCompra?.Equals(sampleListaCompra) == true, "IngredienteCompra.new relates sampleIngredienteCompra and sampleListaCompra.");
            Ensure(sampleIngredienteCompra.IngredienteCatalogo?.Equals(sampleIngrediente) == true, "IngredienteCompra.new relates sampleIngredienteCompra and sampleIngrediente.");
            var sampleIngredienteCompraReadBack = await ingredienteCompraService.ReadOIDAsync(sampleIngredienteCompra.Id);
            Ensure(sampleIngredienteCompraReadBack is not null, "IngredienteCompra read by OID returns the generated entity.");
            var sampleIngredienteCompraReadAll = await ingredienteCompraService.ReadAllAsync();
            Ensure(sampleIngredienteCompraReadAll.Count > 0, "IngredienteCompra read all returns generated data.");
            await ingredienteCompraService.ModifyAsync(sampleIngredienteCompra.Id, 8, new DateTime(2026, 1, 8), true, sampleNevera.Id, sampleListaCompra.Id, sampleIngrediente.Id);
            Ensure(await ingredienteCompraService.ReadOIDIngredienteCompraDefaultAsync(sampleIngredienteCompra.Id) is not null, "IngredienteCompra modify keeps the entity readable.");

            var ingredienteNeveraService = new IngredienteNeveraService(unitOfWork);
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
            Ensure(await ingredienteNeveraService.ReadOIDIngredienteNeveraDefaultAsync(sampleIngredienteNevera.Id) is not null, "IngredienteNevera modify keeps the entity readable.");

            var comentarioService = new ComentarioService(unitOfWork);
            var sampleComentario = await comentarioService.NewAsync(10, "Sample Comentario Titulo", 10, sampleUsuario.Id, sampleReceta.Id);
            Ensure(sampleComentario.Comentador?.Equals(sampleUsuario) == true, "Comentario.new relates sampleComentario and sampleUsuario.");
            Ensure(sampleComentario.Receta?.Equals(sampleReceta) == true, "Comentario.new relates sampleComentario and sampleReceta.");
            var sampleComentarioReadBack = await comentarioService.ReadOIDAsync(sampleComentario.Id);
            Ensure(sampleComentarioReadBack is not null, "Comentario read by OID returns the generated entity.");
            var sampleComentarioReadAll = await comentarioService.ReadAllAsync();
            Ensure(sampleComentarioReadAll.Count > 0, "Comentario read all returns generated data.");
            await comentarioService.ModifyAsync(sampleComentario.Id, 10, "Sample Comentario Titulo", 10);
            Ensure(await comentarioService.ReadOIDComentarioDefaultAsync(sampleComentario.Id) is not null, "Comentario modify keeps the entity readable.");

            var pasoService = new PasoService(unitOfWork);
            var samplePaso = await pasoService.NewAsync("Sample Paso Nombre", "Sample Paso Descripcion", sampleReceta.Id);
            Ensure(samplePaso.Receta?.Equals(sampleReceta) == true, "Paso.new relates samplePaso and sampleReceta.");
            var samplePasoReadBack = await pasoService.ReadOIDAsync(samplePaso.Id);
            Ensure(samplePasoReadBack is not null, "Paso read by OID returns the generated entity.");
            var samplePasoReadAll = await pasoService.ReadAllAsync();
            Ensure(samplePasoReadAll.Count > 0, "Paso read all returns generated data.");
            await pasoService.ModifyAsync(samplePaso.Id, "Sample Paso Nombre", "Sample Paso Descripcion");
            Ensure(await pasoService.ReadOIDPasoDefaultAsync(samplePaso.Id) is not null, "Paso modify keeps the entity readable.");

            // await ingredienteCompraService.DestroyAsync(sampleIngredienteCompra.Id);
            // await ingredienteNeveraService.DestroyAsync(sampleIngredienteNevera.Id);
            // await comentarioService.DestroyAsync(sampleComentario.Id);
            // await pasoService.DestroyAsync(samplePaso.Id);
            // await recetaService.DestroyAsync(sampleReceta.Id);
            // await ingredienteService.DestroyAsync(sampleIngrediente.Id);
            // await chefService.DestroyAsync(sampleChef.Id);
            // await usuarioService.DestroyAsync(sampleUsuario.Id);

            // Ensure(await ingredienteService.ReadOIDIngredienteDefaultAsync(sampleIngrediente.Id) is null, "Ingrediente seed cleanup completed.");
            // Ensure(await usuarioService.ReadOIDUsuarioDefaultAsync(sampleUsuario.Id) is null, "Usuario seed cleanup completed.");
            // Ensure(await chefService.ReadOIDChefDefaultAsync(sampleChef.Id) is null, "Chef seed cleanup completed.");
            // Ensure(await recetaService.ReadOIDRecetaDefaultAsync(sampleReceta.Id) is null, "Receta seed cleanup completed.");
            // Ensure(await ingredienteCompraService.ReadOIDIngredienteCompraDefaultAsync(sampleIngredienteCompra.Id) is null, "IngredienteCompra seed cleanup completed.");
            // Ensure(await ingredienteNeveraService.ReadOIDIngredienteNeveraDefaultAsync(sampleIngredienteNevera.Id) is null, "IngredienteNevera seed cleanup completed.");
            // Ensure(await comentarioService.ReadOIDComentarioDefaultAsync(sampleComentario.Id) is null, "Comentario seed cleanup completed.");
            // Ensure(await pasoService.ReadOIDPasoDefaultAsync(samplePaso.Id) is null, "Paso seed cleanup completed.");
            /*PROTECTED REGION ID(initializeDataMethod) ENABLED START*/
            // Add custom seed data here.

            // ---- Comunidad (Rodrigo): comentarios iniciales con valoraciones variadas ----
            // Debe ejecutarse DESPUÉS del seed de usuarios y recetas del resto del equipo (mantener este bloque
            // el último de la región al integrar). No falla si todavía no hay usuarios o recetas.
            {
                var cmUsuarios = (await unitOfWork.UsuarioRepository.ReadAllAsync()).OrderBy(u => u.Id).ToList();
                var cmRecetas = (await unitOfWork.RecetaRepository.ReadAllAsync()).OrderBy(r => r.Id).ToList();
                // Se ignoran las recetas de ejemplo generadas por HARIA: su comentario de muestra tiene valoración 10
                // (fuera del rango 1-5) y haría fallar la postcondición de la media. Solo se usan recetas reales.
                cmRecetas = cmRecetas.Where(r => !r.Nombre.StartsWith("Sample ", StringComparison.Ordinal)).Take(5).ToList();

                if (cmUsuarios.Count == 0 || cmRecetas.Count == 0)
                {
                    Console.WriteLine("[SKIP] No hay recetas reales (o usuarios): no se siembran comentarios iniciales.");
                }
                else
                {
                    var cmUseCase = new ComentarioUseCase(unitOfWork);
                    var cmService = new ComentarioService(unitOfWork);
                    var cmTextos = new (string Titulo, string Contenido)[]
                    {
                        ("Deliciosa", "Salió perfecta a la primera, la repetiré el fin de semana."),
                        ("Muy fácil de seguir", "Los pasos están claros y los tiempos son justos."),
                        ("Correcta", "Está bien, aunque le añadiría un poco más de sazón."),
                        ("Para repetir", "Gustó a toda la familia. Los ingredientes son fáciles de encontrar."),
                        ("Regular", "Se me quedó algo seca, quizá me pasé de tiempo en el horno."),
                        ("Buena para el día a día", "Rápida y con lo que suele haber en la nevera."),
                        ("No me convenció", "La textura no era la esperada y el resultado fue flojo."),
                        ("Una joya", "Receta de las que se guardan. Presentación muy vistosa.")
                    };
                    // Mezcla de notas altas, medias y bajas para que las medias resultantes sean distintas.
                    var cmValoraciones = new[] { 5, 4, 3, 5, 2, 4, 1, 5 };
                    var cmIdentificador = 100;

                    for (var r = 0; r < cmRecetas.Count; r++)
                    {
                        for (var k = 0; k < 3; k++)
                        {
                            var cmIndice = r * 3 + k;
                            var cmAutor = cmUsuarios[(r + k) % cmUsuarios.Count];
                            var cmValoracion = cmValoraciones[cmIndice % cmValoraciones.Length];
                            var cmTexto = cmTextos[cmIndice % cmTextos.Length];

                            // CustomTransaction: crea el comentario y recalcula Receta.ValoracionMedia.
                            var cmComentario = await cmUseCase.RecalcularValoracionMediaAsync(
                                cmIdentificador++, cmTexto.Titulo, cmValoracion, cmAutor.Id, cmRecetas[r].Id);
                            // Contenido no forma parte del contrato de New: se completa y se guarda aparte.
                            cmComentario.Contenido = cmTexto.Contenido;
                            await cmService.ModifierComentarioDefaultAsync(cmComentario);
                        }
                    }

                    var cmSembrados = await cmService.ReadAllAsync();
                    Ensure(cmSembrados.Count >= cmRecetas.Count * 3, "Comentarios iniciales sembrados.");
                    foreach (var cmReceta in cmRecetas)
                    {
                        var cmRecetaLeida = await unitOfWork.RecetaRepository.ReadAsync(cmReceta.Id);
                        Ensure(cmRecetaLeida is not null && cmRecetaLeida.ValoracionMedia >= 1f && cmRecetaLeida.ValoracionMedia <= 5f,
                            $"Receta '{cmReceta.Nombre}' tiene una valoración media entre 1 y 5.");
                    }
                }
            }
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