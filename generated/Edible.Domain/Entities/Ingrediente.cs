using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Edible.Domain.Enums;

namespace Edible.Domain.Entities;

public partial class Ingrediente
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public TipoIngrediente Tipo { get; set; }
    public int? Calorias { get; set; }
    public float? G_proteinas { get; set; }
    public float? G_carbohidratos { get; set; }
    public float? G_grasas { get; set; }
    public float? G_racion { get; set; }
    public IList<Alergenos> Alergenos { get; set; } = new List<Alergenos>();
    public IList<DietaEspecial> DietaEspecial { get; set; } = new List<DietaEspecial>();
    public IList<LineaReceta> LineasDeReceta { get; set; } = new List<LineaReceta>();
    public IList<IngredienteCompra> IngredienteEnCompra { get; set; } = new List<IngredienteCompra>();
    public IList<IngredienteNevera> IngredienteEnNevera { get; set; } = new List<IngredienteNevera>();

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Ingrediente other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
