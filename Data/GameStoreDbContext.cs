using Microsoft.EntityFrameworkCore;
using GameStore.Api.Models;

namespace GameStore.Api.Data;

public class GameStoreDbContext : DbContext
{
    public GameStoreDbContext(DbContextOptions<GameStoreDbContext> options) : base(options) { }

    public DbSet<Game> Games { get; set; }
    public DbSet<UserCollection> UserCollections { get; set; }
    public DbSet<GameRating> GameRatings { get; set; }
    public DbSet<Blog> Blogs { get; set; }
    public DbSet<GamePrice> GamePrices => Set<GamePrice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserCollection>()
            .HasIndex(x => new { x.UserId, x.GameId })
            .IsUnique();

        modelBuilder.Entity<UserCollection>()
            .HasOne(x => x.Game)
            .WithMany()
            .HasForeignKey(x => x.GameId);

        modelBuilder.Entity<GameRating>()
            .HasIndex(x => new { x.UserId, x.GameId })
            .IsUnique();

        modelBuilder.Entity<GameRating>()
            .HasOne(x => x.Game)
            .WithMany()
            .HasForeignKey(x => x.GameId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.Property(b => b.Title).HasColumnType("text");
            entity.Property(b => b.Content).HasColumnType("text");
            entity.Property(b => b.UpdatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .ValueGeneratedOnAddOrUpdate();
        });

        modelBuilder.Entity<Game>(e =>
        {
            e.HasKey(g => g.Id);
            e.HasMany(g => g.Prices)
             .WithOne(p => p.Game)
             .HasForeignKey(p => p.GameId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GamePrice>(e =>
        {
            e.HasKey(p => p.Id);

            e.Property(p => p.Currency)
             .HasMaxLength(3)
             .IsRequired();

            e.Property(p => p.PricePaise)
             .IsRequired();

            e.Property(p => p.IsActive)
             .HasDefaultValue(true);

            e.Property(p => p.CreatedUtc)
             .HasDefaultValueSql("now()");

            e.HasIndex(p => p.GameId);
            e.HasIndex(p => new { p.GameId, p.Currency });

            // Allow only ONE active price per game/currency
            // Postgres filtered unique index
            e.HasIndex(p => new { p.GameId, p.Currency, p.IsActive })
             .IsUnique()
             .HasFilter("\"IsActive\" = TRUE");
        });

        // 👇 Seed games
        var now = DateTime.UtcNow;

        modelBuilder.Entity<Game>().HasData(
            new Game
            {
                Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), // use fixed Guids
                Title = "Number Guesser",
                Description = "Guess the number between 1 and 100",
                GameUrl = "https://viplavmankar.github.io/Number-Guesser/",
                ThumbnailUrl = "https://github.com/ViplavMankar/Number-Guesser/blob/main/Images/Number%20Guesser.png?raw=true",
                AuthorUserId = Guid.Empty,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Game
            {
                Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"),
                Title = "BMI Calculator",
                Description = "Calculate your Body Mass Index (BMI)",
                GameUrl = "https://viplavmankar.github.io/BMI-Calculator/",
                ThumbnailUrl = "https://github.com/ViplavMankar/BMI-Calculator/blob/main/Screenshot%20from%202025-06-13%2013-07-58.png?raw=true",
                AuthorUserId = Guid.Empty,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Game
            {
                Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"),
                Title = "Pong",
                Description = "Play the classic Pong game",
                GameUrl = "https://viplavmankar.github.io/Pong_Game/",
                ThumbnailUrl = "https://github.com/ViplavMankar/Pong_Game/blob/main/Screenshot%20from%202025-06-14%2011-45-30.png?raw=true",
                AuthorUserId = Guid.Empty,
                CreatedAt = now,
                UpdatedAt = now
            }
        );
    }
}
