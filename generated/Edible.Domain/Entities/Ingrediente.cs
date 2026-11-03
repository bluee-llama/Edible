using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Edible.Domain.Enums;

namespace Edible.Domain.Entities;

public partial class Ingrediente
{
    public virtual int Id { get; set; }
    public virtual string Nombre { get; set; } = string.Empty;
    public virtual TipoIngrediente Tipo { get; set; }
    public virtual int? Calorias { get; set; }
    public virtual float? G_proteinas { get; set; }
    public virtual float? G_carbohidratos { get; set; }
    public virtual float? G_grasas { get; set; }
    public virtual float? G_racion { get; set; }
    public virtual IList<Alergenos> Alergenos { get; set; } = new List<Alergenos>();
    public virtual IList<DietaEspecial> DietaEspecial { get; set; } = new List<DietaEspecial>();
    public virtual IList<LineaReceta> LineasDeReceta { get; set; } = new List<LineaReceta>();
    public virtual IList<IngredienteCompra> IngredienteEnCompra { get; set; } = new List<IngredienteCompra>();
    public virtual IList<IngredienteNevera> IngredienteEnNevera { get; set; } = new List<IngredienteNevera>();

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Ingrediente other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
