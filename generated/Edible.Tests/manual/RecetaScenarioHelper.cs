using System.Reflection;
using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Infrastructure;
using NHibernate;

namespace Edible.Tests.Manual;

public static class RecetaScenarioHelper
{
    // IDs manuales para romper la referencia circular Usuario <-> Nevera/ListaCompra.
    // Empiezan muy alto para no chocar con los IDs que genera NHibernate (hilo).
    private static int _nextId = 100_000;
    private static int NextId() => Interlocked.Increment(ref _nextId);

    // UnitOfWork no expone su sesión; la necesitamos para session.Save(obj, id).
    private static ISession SessionDe(UnitOfWork uow) =>
        (ISession)typeof(UnitOfWork)
            .GetField("_session", BindingFlags.NonPublic | BindingFlags.Instance)!
            .GetValue(uow)!;

    public static async Task<Usuario> CrearUsuarioAsync(UnitOfWork uow, string nombre)
    {
        var usuario = new Usuario { Name = nombre, Password = "pwd", Publico = true };
        await GuardarConNeveraYListaAsync(uow, usuario);
        return usuario;
    }

    public static async Task<Chef> CrearChefAsync(UnitOfWork uow, string nombre)
    {
        var chef = new Chef { Name = nombre, Password = "pwd", Publico = true };
        await GuardarConNeveraYListaAsync(uow, chef);
        return chef;
    }

    private static async Task GuardarConNeveraYListaAsync(UnitOfWork uow, Usuario usuario)
    {
        var session = SessionDe(uow);

        usuario.Id = NextId();
        var nevera = new Nevera { Id = NextId(), Usuario = usuario };
        var lista = new ListaCompra { Id = NextId(), Usuario = usuario };
        usuario.Nevera = nevera;
        usuario.ListaCompra = lista;

        await uow.BeginTransactionAsync("test.GuardarUsuario");
        try
        {
            await session.SaveAsync(usuario, usuario.Id); // Save con id explícito (no SaveOrUpdate)
            await session.SaveAsync(nevera, nevera.Id);
            await session.SaveAsync(lista, lista.Id);
            await uow.CommitAsync();
        }
        catch
        {
            await uow.RollbackAsync();
            throw;
        }
    }

    public static async Task<Ingrediente> CrearIngredienteAsync(UnitOfWork uow, string nombre, params Alergenos[] alergenos)
    {
        var ingrediente = new Ingrediente { Nombre = nombre };
        foreach (var a in alergenos)
            ingrediente.Alergenos.Add(a);
        await uow.IngredienteRepository.SaveAsync(ingrediente);
        return ingrediente;
    }

    public static async Task<Receta> CrearRecetaAsync(
        UnitOfWork uow, Chef chef, string nombre, DietaEspecial dieta,
        float nota, int tiempo, int dificultad, params Ingrediente[] ingredientes)
    {
        var receta = new Receta
        {
            Nombre = nombre,
            Descripcion = "desc",
            DietaEspecial = dieta,
            ValoracionMedia = nota,
            TiempoRealizacion = tiempo,
            Dificultad = dificultad,
            Chef = chef
        };
        foreach (var ing in ingredientes)
            receta.Lineas.Add(new LineaReceta { CantidadNecesaria = 1, IngredienteCatalogo = ing, Receta = receta });

        await uow.RecetaRepository.SaveAsync(receta); // Lineas tiene cascade all-delete-orphan
        return receta;
    }
}