using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

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
else if (Environment.GetEnvironmentVariable("RENDER") != null)
{
    /*Keeping this here for Render.com*/
    // Read the PORT from environment variables (default: 5000)
    var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
    builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
}
await app.MigrateDbAsync();

app.Run();
