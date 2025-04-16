using GameStore.Api.Data;
using GameStore.Api.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var gameStoreConnectionString = string.Empty;
var jwtKey = string.Empty;

if (builder.Environment.IsDevelopment())
{
    gameStoreConnectionString = builder.Configuration.GetConnectionString("GameStoreString");
    if (string.IsNullOrEmpty(gameStoreConnectionString))
    {
        throw new InvalidOperationException("gamestore connection string appsetting value not set.");
    }
    jwtKey = builder.Configuration["Jwt:Key"];
    if (string.IsNullOrEmpty(jwtKey))
    {
        throw new InvalidOperationException("jwtkey appsetting value not set.");
    }
}
else
{
    if (Environment.GetEnvironmentVariable("RENDER") != null)
    {
        var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
        builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

        gameStoreConnectionString = Environment.GetEnvironmentVariable("GAME_STORE_CONNECTION_STRING");
        if (string.IsNullOrEmpty(gameStoreConnectionString))
        {
            throw new InvalidOperationException("DATABASE_URL environment variable is not set.");
        }

        jwtKey = Environment.GetEnvironmentVariable("JWT_AUTH_KEY");
        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT_AUTH_KEY environment variable is not set.");
        }
    }
}
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// builder.Services.AddNpgsql<GameStoreContext>(gameStoreConnectionString);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
// app.MapGamesEndpoints();
// app.MapGenresEndpoints();
// app.MapHealthEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
await app.MigrateDbAsync();

app.Run();
