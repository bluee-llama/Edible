using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Edible.Domain.Entities;

public partial class Comentario
{
    public virtual int Id { get; set; }
    public virtual int User_id { get; set; }
    public virtual string? Imagen { get; set; }
    public virtual string Titulo { get; set; } = string.Empty;
    public virtual string? Contenido { get; set; }
    public virtual int Valoracion { get; set; }
    public virtual Usuario Comentador { get; set; } = null!;
    public virtual Receta Receta { get; set; } = null!;

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Comentario other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
