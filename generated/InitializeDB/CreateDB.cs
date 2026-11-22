using Microsoft.EntityFrameworkCore;
using Edible.Infrastructure.Data;

namespace InitializeDB;

public static class CreateDB
{
    public static async Task RecreateDatabaseAsync(DbContextOptions<EdibleDbContext> options)
    {
        await using var db = new EdibleDbContext(options);
        await db.Database.EnsureDeletedAsync();
    }

    public static async Task CreateSchemaAsync(DbContextOptions<EdibleDbContext> options)
    {
        await using var db = new EdibleDbContext(options);
        await db.Database.EnsureCreatedAsync();
    }
}