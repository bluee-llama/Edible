using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Edible.Domain.Entities;

public partial class IngredienteNevera
{
    public virtual int Id { get; set; }
    public virtual int Cantidad { get; set; }
    public virtual DateTime FechaCaducidad { get; set; }
    public virtual Ingrediente IngredienteCatalogo { get; set; } = null!;
    public virtual Nevera Nevera { get; set; } = null!;
    public virtual Receta Receta { get; set; } = null!;

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is IngredienteNevera other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
