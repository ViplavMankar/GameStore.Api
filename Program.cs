using GameStore.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GameStore.Api.Services;
using Microsoft.OpenApi.Models;
using GameStore.Api.Models;

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
    builder.Services.AddHttpClient("AuthService", client =>
    {
        client.BaseAddress = new Uri("https://localhost:7168/");
    })
    .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
    {
        ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
    });
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
        builder.Services.AddHttpClient("AuthService", client =>
        {
            client.BaseAddress = Environment.GetEnvironmentVariable("AUTH_SERVICE_URL") != null
                ? new Uri(Environment.GetEnvironmentVariable("AUTH_SERVICE_URL"))
                : throw new Exception("Environment variable not set.");
        });
    }
}
builder.Services.AddControllers();
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

builder.Services.AddDbContext<GameStoreDbContext>(options => options.UseNpgsql(gameStoreConnectionString));

builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ICollectionService, CollectionService>();
builder.Services.AddScoped<IGameRatingService, GameRatingService>();
builder.Services.AddScoped<IBlogService, BlogService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by space and JWT token"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
await app.MigrateDbAsync();

app.Run();
