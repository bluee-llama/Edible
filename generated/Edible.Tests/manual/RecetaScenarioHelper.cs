using Edible.Domain.Entities;
using Edible.Domain.Enums;
using Edible.Infrastructure;

namespace Edible.Tests.Manual;

public static class RecetaScenarioHelper
{
    public static async Task<Usuario> CrearUsuarioAsync(UnitOfWork uow, string nombre)
    {
        var nevera = new Nevera();
        var lista = new ListaCompra();
        var usuario = new Usuario
        {
            Name = nombre,
            Password = "pwd",
            Publico = true,
            Nevera = nevera,
            ListaCompra = lista
        };
        nevera.Usuario = usuario;
        lista.Usuario = usuario;

        await uow.UsuarioRepository.SaveAsync(usuario);
        return usuario;
    }

    public static async Task<Chef> CrearChefAsync(UnitOfWork uow, string nombre)
    {
        var nevera = new Nevera();
        var lista = new ListaCompra();
        var chef = new Chef
        {
            Name = nombre,
            Password = "pwd",
            Publico = true,
            Nevera = nevera,
            ListaCompra = lista
        };
        nevera.Usuario = chef;
        lista.Usuario = chef;

        await uow.ChefRepository.SaveAsync(chef);
        return chef;
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

        await uow.RecetaRepository.SaveAsync(receta);
        return receta;
    }
}