namespace Edible.Web.ViewModels;

// ViewModel manual de la operación transaccional Nevera.recorrerListaCompra (CustomTransaction).
// Muestra los ingredientes en la lista de compra (comprados vs pendientes) y el stock actual en la nevera
// para comprobar visualmente el efecto antes y después de la transacción.
public sealed class RecorrerListaCompraViewModel
{
    public int? UsuarioId { get; set; }
    public string? UsuarioNombre { get; set; }

    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> UsuarioOptions { get; set; } = new();

    public List<ItemCompraResumen> ItemsListaCompra { get; set; } = new();
    public List<ItemNeveraResumen> ItemsNevera { get; set; } = new();

    public int TotalComprados => ItemsListaCompra.Count(i => i.Comprado);

    public sealed record ItemCompraResumen(int Id, string IngredienteNombre, int Cantidad, bool Comprado);
    public sealed record ItemNeveraResumen(int Id, string IngredienteNombre, int Cantidad, DateTime FechaCaducidad);
}
