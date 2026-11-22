// HARIA-WEB GENERATED sha256:08483d014ca5d29ba5d44a7b483d244019ca64c0b4b3520779cf3afbcba42e26
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace Edible.Web.Configuration;

public static class HariaApplicationRegistration
{
    public static IServiceCollection AddHariaApplication(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var connectionString = RequireConnectionString(configuration, environment);
        services.AddDbContext<Edible.Infrastructure.Data.EdibleDbContext>(options =>
            options.UseSqlite(connectionString));
        services.AddScoped<Edible.Application.Abstractions.IHariaPasswordHasher, Edible.Infrastructure.Security.HariaPasswordHasher>();
        services.AddScoped<Edible.Application.Abstractions.IHariaTokenGenerator, Edible.Infrastructure.Security.HariaTokenGenerator>();
        services.AddScoped<Edible.Application.Abstractions.IUsuarioRepository, Edible.Infrastructure.Repositories.UsuarioRepository>();
        services.AddScoped<Edible.Application.Abstractions.IUnitOfWorkFactory, Edible.Infrastructure.UnitOfWorkFactory>();
        services.AddScoped<Edible.Application.Services.ChefService>();
        services.AddScoped<Edible.Application.Services.NeveraService>();
        services.AddScoped<Edible.Application.Services.ListaCompraService>();
        services.AddScoped<Edible.Application.Services.RecetaService>();
        services.AddScoped<Edible.Application.Services.IngredienteService>();
        services.AddScoped<Edible.Application.Services.IngredienteCompraService>();
        services.AddScoped<Edible.Application.Services.IngredienteNeveraService>();
        services.AddScoped<Edible.Application.Services.ComentarioService>();
        services.AddScoped<Edible.Application.Services.PasoService>();
        services.AddScoped<Edible.Application.Services.UsuarioService>();
        return services;
    }

    private static string RequireConnectionString(IConfiguration configuration, IHostEnvironment environment)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Configure ConnectionStrings:DbConnection in appsettings.Haria.json, user-secrets, or the ConnectionStrings__DbConnection environment variable.");
        }

        var sqlite = new System.Data.Common.DbConnectionStringBuilder { ConnectionString = connectionString };
        var dataSourceKey = sqlite.Keys.Cast<string>().FirstOrDefault(key =>
            string.Equals(key.Replace(" ", ""), "DataSource", StringComparison.OrdinalIgnoreCase)
            || string.Equals(key, "Filename", StringComparison.OrdinalIgnoreCase));
        if (dataSourceKey is null)
        {
            throw new InvalidOperationException("The SQLite connection string must specify Data Source or Filename.");
        }
        var dataSource = Convert.ToString(sqlite[dataSourceKey]) ?? string.Empty;
        if (dataSource.Length == 0 || dataSource.StartsWith("file:", StringComparison.OrdinalIgnoreCase) || Path.IsPathRooted(dataSource)
            || string.Equals(dataSource, ":memory:", StringComparison.OrdinalIgnoreCase)
            || (sqlite.TryGetValue("Mode", out var mode) && string.Equals(Convert.ToString(mode), "Memory", StringComparison.OrdinalIgnoreCase)))
        {
            return sqlite.ConnectionString;
        }

        sqlite[dataSourceKey] = Path.GetFullPath(Path.Combine(environment.ContentRootPath, dataSource));
        return sqlite.ConnectionString;
    }
}
