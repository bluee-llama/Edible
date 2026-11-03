using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Edible.Domain.Enums;

namespace Edible.Domain.Entities;

public partial class Usuario
{
    public virtual int Id { get; set; }
    public virtual string Name { get; set; } = string.Empty;
    public virtual string Password { get; set; } = string.Empty;
    public virtual bool Publico { get; set; }
    public virtual DietaEspecial Dieta { get; set; }
    public virtual IList<Comentario> Comentario { get; set; } = new List<Comentario>();
    public virtual Nevera Nevera { get; set; } = null!;
    public virtual ListaCompra ListaCompra { get; set; } = null!;
    public virtual IList<Receta> RecetasSugeridas { get; set; } = new List<Receta>();

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Usuario other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
