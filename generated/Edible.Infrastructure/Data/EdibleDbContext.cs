using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using Edible.Domain.Entities;
using Edible.Domain.Enums;

namespace Edible.Infrastructure.Data;

public sealed class EdibleDbContext : DbContext
{
    public EdibleDbContext(DbContextOptions<EdibleDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Chef> Chefs => Set<Chef>();
    public DbSet<Nevera> Neveras => Set<Nevera>();
    public DbSet<ListaCompra> ListaCompras => Set<ListaCompra>();
    public DbSet<Receta> Recetas => Set<Receta>();
    public DbSet<LineaReceta> LineaRecetas => Set<LineaReceta>();
    public DbSet<Ingrediente> Ingredientes => Set<Ingrediente>();
    public DbSet<IngredienteCompra> IngredienteCompras => Set<IngredienteCompra>();
    public DbSet<IngredienteNevera> IngredienteNeveras => Set<IngredienteNevera>();
    public DbSet<Comentario> Comentarios => Set<Comentario>();
    public DbSet<Paso> Pasos => Set<Paso>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Name)
                .HasColumnName("name")
                .IsRequired();
            entity.Property(e => e.Password)
                .HasColumnName("password")
                .IsRequired();
            entity.Property(e => e.Publico)
                .HasColumnName("publico")
                .IsRequired();
            entity.Property(e => e.Dieta)
                .HasColumnName("dieta")
                .HasConversion<int>()
                .IsRequired();
        });

        modelBuilder.Entity<Chef>(entity =>
        {
            entity.ToTable("Chef");
            entity.HasBaseType<Usuario>();
        });

        modelBuilder.Entity<Nevera>(entity =>
        {
            entity.ToTable("Nevera");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<ListaCompra>(entity =>
        {
            entity.ToTable("ListaCompra");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
        });

        modelBuilder.Entity<Receta>(entity =>
        {
            entity.ToTable("Receta");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre)
                .HasColumnName("nombre")
                .IsRequired();
            entity.Property(e => e.Descripcion)
                .HasColumnName("descripcion")
                .IsRequired();
            entity.Property(e => e.DietaEspecial)
                .HasColumnName("dietaEspecial")
                .HasConversion<int>()
                .IsRequired();
            entity.Property(e => e.ValoracionMedia)
                .HasColumnName("valoracionMedia")
                .IsRequired();
            entity.Property(e => e.TiempoRealizacion)
                .HasColumnName("tiempoRealizacion")
                .IsRequired();
            entity.Property(e => e.Dificultad)
                .HasColumnName("dificultad")
                .IsRequired();
        });

        modelBuilder.Entity<LineaReceta>(entity =>
        {
            entity.ToTable("LineaReceta");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.CantidadNecesaria)
                .HasColumnName("cantidadNecesaria")
                .IsRequired();
        });

        modelBuilder.Entity<Ingrediente>(entity =>
        {
            entity.ToTable("Ingrediente");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre)
                .HasColumnName("nombre")
                .IsRequired();
            entity.Property(e => e.Tipo)
                .HasColumnName("tipo")
                .HasConversion<int>()
                .IsRequired();
            entity.Property(e => e.Calorias)
                .HasColumnName("calorias")
                .IsRequired(false);
            entity.Property(e => e.G_proteinas)
                .HasColumnName("g_proteinas")
                .IsRequired(false);
            entity.Property(e => e.G_carbohidratos)
                .HasColumnName("g_carbohidratos")
                .IsRequired(false);
            entity.Property(e => e.G_grasas)
                .HasColumnName("g_grasas")
                .IsRequired(false);
            entity.Property(e => e.G_racion)
                .HasColumnName("g_racion")
                .IsRequired(false);
            entity.Property(e => e.Alergenos)
                .HasColumnName("alergenos")
                .HasColumnType("TEXT")
                .HasConversion(
                    value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                    value => JsonSerializer.Deserialize<List<Alergenos>>(value, (JsonSerializerOptions?)null) ?? new List<Alergenos>());
            entity.Property(e => e.Alergenos).Metadata.SetValueComparer(
                new ValueComparer<IList<Alergenos>>(
                    (left, right) => ReferenceEquals(left, right) || (left != null && right != null && left.SequenceEqual(right)),
                    value => value == null ? 0 : value.Aggregate(0, (hash, item) => HashCode.Combine(hash, EqualityComparer<Alergenos>.Default.GetHashCode(item))),
                    value => value == null ? new List<Alergenos>() : value.ToList()));
            entity.Property(e => e.DietaEspecial)
                .HasColumnName("dietaEspecial")
                .HasColumnType("TEXT")
                .HasConversion(
                    value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                    value => JsonSerializer.Deserialize<List<DietaEspecial>>(value, (JsonSerializerOptions?)null) ?? new List<DietaEspecial>());
            entity.Property(e => e.DietaEspecial).Metadata.SetValueComparer(
                new ValueComparer<IList<DietaEspecial>>(
                    (left, right) => ReferenceEquals(left, right) || (left != null && right != null && left.SequenceEqual(right)),
                    value => value == null ? 0 : value.Aggregate(0, (hash, item) => HashCode.Combine(hash, EqualityComparer<DietaEspecial>.Default.GetHashCode(item))),
                    value => value == null ? new List<DietaEspecial>() : value.ToList()));
        });

        modelBuilder.Entity<IngredienteCompra>(entity =>
        {
            entity.ToTable("IngredienteCompra");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Cantidad)
                .HasColumnName("cantidad")
                .IsRequired();
            entity.Property(e => e.Comprado)
                .HasColumnName("comprado")
                .IsRequired();
        });

        modelBuilder.Entity<IngredienteNevera>(entity =>
        {
            entity.ToTable("IngredienteNevera");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Cantidad)
                .HasColumnName("cantidad")
                .IsRequired();
            entity.Property(e => e.FechaCaducidad)
                .HasColumnName("fechaCaducidad")
                .HasColumnType("date")
                .IsRequired();
        });

        modelBuilder.Entity<Comentario>(entity =>
        {
            entity.ToTable("Comentario");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.User_id)
                .HasColumnName("user_id")
                .IsRequired();
            entity.Property(e => e.Imagen)
                .HasColumnName("imagen")
                .IsRequired(false);
            entity.Property(e => e.Titulo)
                .HasColumnName("titulo")
                .IsRequired();
            entity.Property(e => e.Contenido)
                .HasColumnName("contenido")
                .IsRequired(false);
            entity.Property(e => e.Valoracion)
                .HasColumnName("valoracion")
                .IsRequired();
        });

        modelBuilder.Entity<Paso>(entity =>
        {
            entity.ToTable("Paso");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();
            entity.Property(e => e.Nombre)
                .HasColumnName("nombre")
                .IsRequired();
            entity.Property(e => e.Descripcion)
                .HasColumnName("descripcion")
                .IsRequired();
        });

        modelBuilder.Entity<Chef>()
            .HasMany(e => e.Receta)
            .WithOne(e => e.Chef)
            .HasForeignKey("ChefId")
            .IsRequired();

        modelBuilder.Entity<Receta>()
            .HasMany(e => e.Pasos)
            .WithOne(e => e.Receta)
            .HasForeignKey("RecetaId")
            .IsRequired();

        modelBuilder.Entity<Usuario>()
            .HasMany(e => e.Comentario)
            .WithOne(e => e.Comentador)
            .HasForeignKey("ComentadorId")
            .IsRequired();

        modelBuilder.Entity<Receta>()
            .HasMany(e => e.Lineas)
            .WithOne(e => e.Receta)
            .HasForeignKey("RecetaId")
            .IsRequired();

        modelBuilder.Entity<Ingrediente>()
            .HasMany(e => e.LineasDeReceta)
            .WithOne(e => e.IngredienteCatalogo)
            .HasForeignKey("IngredienteCatalogoId")
            .IsRequired();

        modelBuilder.Entity<Usuario>()
            .HasOne(e => e.Nevera)
            .WithOne(e => e.Usuario)
            .HasForeignKey<Usuario>("NeveraId")
            .IsRequired();

        modelBuilder.Entity<Receta>()
            .HasMany(e => e.Comentarios)
            .WithOne(e => e.Receta)
            .HasForeignKey("RecetaId")
            .IsRequired();

        modelBuilder.Entity<Usuario>()
            .HasOne(e => e.ListaCompra)
            .WithOne(e => e.Usuario)
            .HasForeignKey<Usuario>("ListaCompraId")
            .IsRequired();

        modelBuilder.Entity<Nevera>()
            .HasMany(e => e.IngredienteEnLaLista)
            .WithOne(e => e.Nevera)
            .HasForeignKey("NeveraId")
            .IsRequired();

        modelBuilder.Entity<ListaCompra>()
            .HasMany(e => e.IngredienteEnLaLista)
            .WithOne(e => e.ListaCompra)
            .HasForeignKey("ListaCompraId")
            .IsRequired();

        modelBuilder.Entity<Ingrediente>()
            .HasMany(e => e.IngredienteEnCompra)
            .WithOne(e => e.IngredienteCatalogo)
            .HasForeignKey("IngredienteCatalogoId")
            .IsRequired();

        modelBuilder.Entity<Ingrediente>()
            .HasMany(e => e.IngredienteEnNevera)
            .WithOne(e => e.IngredienteCatalogo)
            .HasForeignKey("IngredienteCatalogoId")
            .IsRequired();

        modelBuilder.Entity<Nevera>()
            .HasMany(e => e.IngredienteEnLaNevera)
            .WithOne(e => e.Nevera)
            .HasForeignKey("NeveraId")
            .IsRequired();

        modelBuilder.Entity<Receta>()
            .HasMany(e => e.IngredientesDisponibles)
            .WithOne(e => e.Receta)
            .HasForeignKey("RecetaId")
            .IsRequired(false);

        modelBuilder.Entity<Usuario>()
            .HasMany(e => e.RecetasSugeridas)
            .WithMany(e => e.UsuariosSugeridos);

    }
}
