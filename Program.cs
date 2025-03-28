using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

var connString = builder.Configuration.GetConnectionString("GameStore");
builder.Services.AddSqlite<GameStoreContext>(connString);

var app = builder.Build();

app.MapGamesEndpoints();
app.MapGenresEndpoints();

if (Environment.GetEnvironmentVariable("RENDER") != null)
{
    /*Keeping this here for Render.com*/
}
await app.MigrateDbAsync();

app.Run();
