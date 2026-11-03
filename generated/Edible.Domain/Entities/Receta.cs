using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Edible.Domain.Enums;

namespace Edible.Domain.Entities;

public partial class Receta
{
    public virtual int Id { get; set; }
    public virtual string Nombre { get; set; } = string.Empty;
    public virtual string Descripcion { get; set; } = string.Empty;
    public virtual DietaEspecial DietaEspecial { get; set; }
    public virtual float ValoracionMedia { get; set; }
    public virtual int TiempoRealizacion { get; set; }
    public virtual int Dificultad { get; set; }
    public virtual Chef Chef { get; set; } = null!;
    public virtual IList<Paso> Pasos { get; set; } = new List<Paso>();
    public virtual IList<LineaReceta> Lineas { get; set; } = new List<LineaReceta>();
    public virtual IList<Comentario> Comentarios { get; set; } = new List<Comentario>();
    public virtual IList<IngredienteNevera> IngredientesDisponibles { get; set; } = new List<IngredienteNevera>();
    public virtual IList<Usuario> UsuariosSugeridos { get; set; } = new List<Usuario>();

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Receta other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
