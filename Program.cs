using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Endpoints;
using Microsoft.EntityFrameworkCore;
// using Npgsql;

var builder = WebApplication.CreateBuilder(args);

var gameStoreConnectionString = "";

if (builder.Environment.IsDevelopment())
{
    if (builder.Configuration.GetConnectionString("GameStoreString") == null)
    {
        throw new InvalidOperationException(" is not set.");
    }
    gameStoreConnectionString = builder.Configuration.GetConnectionString("GameStoreString");
    if (string.IsNullOrEmpty(gameStoreConnectionString))
    {
        throw new InvalidOperationException("appsetting value not set.");
    }
}
else
{
    if (Environment.GetEnvironmentVariable("RENDER") != null)
    {
        // Read the PORT from environment variables (default: 5000)
        var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
        gameStoreConnectionString = Environment.GetEnvironmentVariable("GAME_STORE_CONNECTION_STRING");

        if (string.IsNullOrEmpty(gameStoreConnectionString))
        {
            throw new InvalidOperationException("DATABASE_URL environment variable is not set.");
        }
    }
}

builder.Services.AddNpgsql<GameStoreContext>(gameStoreConnectionString);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGamesEndpoints();
app.MapGenresEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// to Apply migrations use this custom Middleware below
await app.MigrateDbAsync();

app.Run();
