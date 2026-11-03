using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Edible.Domain.Entities;

public partial class ListaCompra
{
    public virtual int Id { get; set; }
    public virtual Usuario Usuario { get; set; } = null!;
    public virtual IList<IngredienteCompra> IngredienteEnLaLista { get; set; } = new List<IngredienteCompra>();

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is ListaCompra other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
