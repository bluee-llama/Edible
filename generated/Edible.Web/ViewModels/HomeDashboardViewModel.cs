using Edible.Domain.Enums;

namespace Edible.Web.ViewModels;

/// <summary>Resumen de una receta para tarjetas y listados (Home, perfiles). No es un formulario.</summary>
public sealed class RecetaResumenViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string ChefNombre { get; set; } = string.Empty;
    public DietaEspecial Dieta { get; set; }
    public float ValoracionMedia { get; set; }
    public int TiempoRealizacion { get; set; }
    public int Dificultad { get; set; }
}

public sealed class ChefDestacadoViewModel
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int NumRecetas { get; set; }
    public float NotaMedia { get; set; }
}

public sealed class ComentarioResumenViewModel
{
    public int Id { get; set; }
    public int RecetaId { get; set; }
    public string RecetaNombre { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public int Valoracion { get; set; }
    public string Extracto { get; set; } = string.Empty;
    public bool TieneImagen { get; set; }
}

/// <summary>Portada de Usuario / Chef (CU-06, CU-09; REQ-05, REQ-14, REQ-15, REQ-16).</summary>
public sealed class HomeDashboardViewModel
{
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public DietaEspecial Dieta { get; set; }
    public bool EsChef { get; set; }
    public int? NeveraId { get; set; }

    /// <summary>true si no se han podido calcular las recetas cocinables (se avisa sin romper la portada).</summary>
    public bool ErrorCocinables { get; set; }

    public List<RecetaResumenViewModel> Cocinables { get; set; } = new();
    public List<RecetaResumenViewModel> PorDieta { get; set; } = new();
    public List<RecetaResumenViewModel> MejorValoradas { get; set; } = new();
    public List<RecetaResumenViewModel> Recientes { get; set; } = new();
    public List<RecetaResumenViewModel> Compartidas { get; set; } = new();
    public List<ChefDestacadoViewModel> ChefsDestacados { get; set; } = new();
}

/// <summary>Datos extra de un perfil (CU-03, REQ-02): recetas publicadas, guardadas y comentarios.</summary>
public sealed class PerfilExtrasViewModel
{
    public int UsuarioId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public bool Publico { get; set; }
    public bool EsChef { get; set; }
    public DietaEspecial Dieta { get; set; }

    /// <summary>true si se han cargado los datos privados (sugeridas y comentarios).</summary>
    public bool IncluyePrivado { get; set; }

    public List<RecetaResumenViewModel> Publicadas { get; set; } = new();
    public List<RecetaResumenViewModel> Sugeridas { get; set; } = new();
    public List<ComentarioResumenViewModel> Comentarios { get; set; } = new();
}
