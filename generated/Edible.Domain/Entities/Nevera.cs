using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Edible.Domain.Entities;

public partial class Nevera
{
    public int Id { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public IList<IngredienteCompra> IngredienteEnLaLista { get; set; } = new List<IngredienteCompra>();
    public IList<IngredienteNevera> IngredienteEnLaNevera { get; set; } = new List<IngredienteNevera>();

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Nevera other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
