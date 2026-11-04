using Edible.Application.Abstractions;
using Edible.Application.Exceptions;
using Edible.Application.Services;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

        /*PROTECTED REGION ID(usingEdible.Application.UseCases_Nevera_recorrerListaCompra) ENABLED START*/
        using System;
        using System.Linq;
        /*PROTECTED REGION END*/

namespace Edible.Application.UseCases.Nevera;

public sealed partial class NeveraUseCase
{
    public async Task<global::Edible.Domain.Entities.Nevera> RecorrerListaCompraAsync(int p_usuario_oid)
    {
        /*PROTECTED REGION ID(Edible.Application.UseCases_Nevera_recorrerListaCompra) ENABLED START*/
        // Resolve external infrastructure services with _utils!.GetRequiredService<TService>() when this use case receives IUnitOfWorkUtils.
        NeveraService neveraService = null!;
        global::Edible.Domain.Entities.Nevera result = default(global::Edible.Domain.Entities.Nevera)!;

        try
        {
            await _unitOfWork.BeginTransactionAsync("NeveraUseCase.RecorrerListaCompraAsync");
            neveraService = new NeveraService(_unitOfWork);

            // Write here your custom transaction code...
            var usuario = await _unitOfWork.UsuarioRepository.ReadAsync(p_usuario_oid) ?? throw new ModelException($"El usuario con ID {p_usuario_oid} no existe.");

            var todasNevera = await _unitOfWork.NeveraRepository.ReadAllAsync();
            var nevera = todasNevera.FirstOrDefault(n => n.Usuario != null && n.Usuario.Id == p_usuario_oid) ?? usuario.Nevera ?? throw new ModelException($"El usuario con ID {p_usuario_oid} no tiene una nevera asociada.");

            var todasListas = await _unitOfWork.ListaCompraRepository.ReadAllAsync();
            var listaCompra = todasListas.FirstOrDefault(l => l.Usuario != null && l.Usuario.Id == p_usuario_oid) ?? usuario.ListaCompra ?? throw new ModelException($"El usuario con ID {p_usuario_oid} no tiene una lista de compra asociada.");

            var todosIngredientesCompra = await _unitOfWork.IngredienteCompraRepository.ReadAllAsync();
            var ingredientesComprados = todosIngredientesCompra.Where(i => i.ListaCompra != null && i.ListaCompra.Id == listaCompra.Id && i.Comprado).ToList();

            if (ingredientesComprados.Count == 0)
            {
                throw new ModelException($"No hay ingredientes comprados en la lista de compra del usuario con ID {p_usuario_oid}.");
            }

            var todosIngredientesNevera = await _unitOfWork.IngredienteNeveraRepository.ReadAllAsync();
            var ingredientesNevera = todosIngredientesNevera.Where(i => i.Nevera != null && i.Nevera.Id == nevera.Id).ToList();

            foreach (var ingrediente in ingredientesComprados)
            {
                var existente = ingredientesNevera.FirstOrDefault(i => i.IngredienteCatalogo != null && ingrediente.IngredienteCatalogo != null && i.IngredienteCatalogo.Id == ingrediente.IngredienteCatalogo.Id);

                if (existente != null)
                {
                    existente.Cantidad += ingrediente.Cantidad;
                    await _unitOfWork.IngredienteNeveraRepository.SaveAsync(existente);
                }
                else
                {
                    var nuevoIngrediente = new IngredienteNevera
                    {
                        Cantidad = ingrediente.Cantidad,
                        FechaCaducidad = DateTime.UtcNow.AddDays(7),
                        IngredienteCatalogo = ingrediente.IngredienteCatalogo,
                        Nevera = nevera
                    };
                    await _unitOfWork.IngredienteNeveraRepository.SaveAsync(nuevoIngrediente);
                    ingredientesNevera.Add(nuevoIngrediente);
                }

                await _unitOfWork.IngredienteCompraRepository.DeleteAsync(ingrediente);

            }

            result = nevera;
            
            // Use neveraService, _unitOfWork.NeveraRepository or _utils!.GetRequiredService<TService>() as the generated CP equivalent.

            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            throw new ModelException("Application error while executing use case Nevera.recorrerListaCompra.", ex);
        }

        return result;
        /*PROTECTED REGION END*/
    }
}
