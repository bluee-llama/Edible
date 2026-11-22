using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Edible.Domain.Enums;

namespace Edible.Domain.Entities;

public partial class Receta
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DietaEspecial DietaEspecial { get; set; }
    public float ValoracionMedia { get; set; }
    public int TiempoRealizacion { get; set; }
    public int Dificultad { get; set; }
    public Chef Chef { get; set; } = null!;
    public IList<Paso> Pasos { get; set; } = new List<Paso>();
    public IList<LineaReceta> Lineas { get; set; } = new List<LineaReceta>();
    public IList<Comentario> Comentarios { get; set; } = new List<Comentario>();
    public IList<IngredienteNevera> IngredientesDisponibles { get; set; } = new List<IngredienteNevera>();
    public IList<Usuario> UsuariosSugeridos { get; set; } = new List<Usuario>();

    private bool HasDefaultIdentity() => EqualityComparer<int>.Default.Equals(Id, default);

    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || obj is Receta other && !HasDefaultIdentity() && !other.HasDefaultIdentity() && EqualityComparer<int>.Default.Equals(Id, other.Id);
    public override int GetHashCode() => HasDefaultIdentity() ? RuntimeHelpers.GetHashCode(this) : EqualityComparer<int>.Default.GetHashCode(Id);
}
