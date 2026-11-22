using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Edible.Domain.Enums;

namespace Edible.Domain.Entities;

public partial class Usuario
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool Publico { get; set; }
    public DietaEspecial Dieta { get; set; }
    public IList<Comentario> Comentario { get; set; } = new List<Comentario>();
    public Nevera Nevera { get; set; } = null!;
    public ListaCompra ListaCompra { get; set; } = null!;
    public IList<Receta> RecetasSugeridas { get; set; } = new List<Receta>();

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Usuario other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
