using System;
using GameStore.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class HealthEndpoints
{
    public static RouteGroupBuilder MapHealthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("health");
        group.MapGet("/", async (GameStoreContext dbContext) =>
        {
            try
            {
                await dbContext.Database.ExecuteSqlAsync($"SELECT * FROM public.\"Genres\" LIMIT 1;");
                return Results.Ok(new { status = "Healthy", database = "Connected" });
            }
            catch
            {
                return Results.Problem("Database connection failed", statusCode: 500);
            }
        });
        return group;
    }

}
