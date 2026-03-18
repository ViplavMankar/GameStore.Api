using GameStore.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using GameStore.Api.Services;
using GameStore.Api.DTOs;
using Microsoft.OpenApi.Models;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

var gameStoreConnectionString = string.Empty;
var jwtKey = string.Empty;
var gamestore_base_url = string.Empty;

// builder.Services.Configure<GeminiOptions>(builder.Configuration.GetSection("Gemini"));
builder.Services.Configure<GeminiOptions>(options =>
{
    options.ApiKey = builder.Configuration["Gemini:ApiKey"]
                      ?? Environment.GetEnvironmentVariable("GEMINI_API_KEY");

    options.Model = builder.Configuration["Gemini:Model"]
                      ?? Environment.GetEnvironmentVariable("GEMINI_MODEL");

    var maxTokensStr = builder.Configuration["Gemini:MaxTokens"]
                      ?? Environment.GetEnvironmentVariable("GEMINI_MAX_OUTPUT_TOKENS");
    if (int.TryParse(maxTokensStr, out var maxTokens))
    {
        options.MaxOutputTokens = maxTokens;
    }
    else
    {
        options.MaxOutputTokens = 2048; // default fallback
    }

    var temperatureStr = builder.Configuration["Gemini:Temperature"]
                    ?? Environment.GetEnvironmentVariable("GEMINI_TEMPERATURE");
    if (double.TryParse(maxTokensStr, out var temperature))
    {
        options.Temperature = temperature;
    }
    else
    {
        options.Temperature = 0.7; // default fallback
    }
});

// Typed HttpClient for Google AI Studio (Gemini)
builder.Services.AddHttpClient<GeminiApiClient>((sp, http) =>
{
    var opts = sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<GeminiOptions>>().Value;

    http.BaseAddress = new Uri("https://generativelanguage.googleapis.com/");
    // AI Studio keys authenticate with an API key; using header keeps URLs clean.
    http.DefaultRequestHeaders.Add("x-goog-api-key", opts.ApiKey);
    http.DefaultRequestHeaders.Accept.Add(
        new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
    http.Timeout = TimeSpan.FromSeconds(120);
});

// builder.Services.Configure<AwsSettings>(
//     builder.Configuration.GetSection("AWS"));

builder.Services.Configure<AwsSettings>(options =>
{
    options.Region = builder.Configuration["AWS:Region"]
                      ?? Environment.GetEnvironmentVariable("AWS_REGION");

    options.BucketName = builder.Configuration["AWS:BucketName"]
                      ?? Environment.GetEnvironmentVariable("AWS_BUCKET_NAME");

    options.AccessKey = builder.Configuration["AWS:AccessKey"]
                      ?? Environment.GetEnvironmentVariable("AWS_ACCESS_KEY");

    options.SecretKey = builder.Configuration["AWS:SecretKey"]
                      ?? Environment.GetEnvironmentVariable("AWS_SECRET_KEY");
});

// Your existing app service that calls the client
builder.Services.AddScoped<AiRewriteService>();

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
    gamestore_base_url = "https://localhost:7051";
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
        gamestore_base_url = Environment.GetEnvironmentVariable("GAMESTORE_BASE_URL") != null
            ? Environment.GetEnvironmentVariable("GAMESTORE_BASE_URL")
            : throw new Exception("Environment variable not set.");
    }
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowGameStoreWeb",
        policy => policy
            .WithOrigins(gamestore_base_url) // your Blazor URL
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});
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
builder.Services.AddScoped<IPricingService, PricingService>();
builder.Services.AddScoped<IAiRewriteService, AiRewriteService>();
builder.Services.AddScoped<IS3Service, S3Service>();
builder.Services.AddScoped<ITrendingGameService, TrendingGameService>();
builder.Services.AddScoped<IProfileService, ProfileService>();

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

app.UseCors("AllowGameStoreWeb");
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