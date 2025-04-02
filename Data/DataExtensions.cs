using System;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtensions
{
    public static async Task MigrateDbAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();

        try
        {
            // Log connection string for debugging purposes
            var conn = dbContext.Database.GetDbConnection();
            var connectionString = conn.ConnectionString;
            Console.WriteLine($"Using connection string: {connectionString}");

            // Attempt migration
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Database migration succeeded.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Migration failed: {ex.Message}");
        }
    }
}
